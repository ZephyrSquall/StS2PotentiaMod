using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterAutoPostPlayPhaseEntered))]
internal class AutoPlayAtEndOfTurnPatch
{
    [HarmonyPostfix]
    private static void AfterAutoPostPlayPhaseEnteredPostfix(ref Task __result,
        HookPlayerChoiceContext playerChoiceContext, ICombatState combatState, Player player)
    {
        __result = AsyncWrapper(__result, playerChoiceContext, combatState, player);
    }

    private static async Task AsyncWrapper(Task originalTask, HookPlayerChoiceContext playerChoiceContext,
        ICombatState combatState, Player player)
    {
        await originalTask;

        if (!LocalContext.NetId.HasValue) return;

        // Cards with Auto cannot be auto-played while iterating the PileType.Hand.GetPile(player).Cards loop, because
        // this modifies the collection of cards in Hand while it is being enumerated, crashing the game. Instead, 
        // create a new List of cards with Auto that need to be auto-played, then iterate this list to auto-play them.
        List<CardModel> autoCardsToBeAutoPlayed = [];
        foreach (CardModel cardModel in PileType.Hand.GetPile(player).Cards)
        {
            if (cardModel.IsAutoThisTurn())
            {
                autoCardsToBeAutoPlayed.Add(cardModel);
            }
        }

        foreach (CardModel cardModel in autoCardsToBeAutoPlayed)
        {
            playerChoiceContext.PushModel(cardModel);
            AutoTracker.IsBeingAutoPlayedByAuto[cardModel] = true;
            await CardCmd.AutoPlay(playerChoiceContext, cardModel, null);
            AutoTracker.IsBeingAutoPlayedByAuto[cardModel] = false;
            cardModel.InvokeExecutionFinished();
            playerChoiceContext.PopModel(cardModel);
        }
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyCardPlayResultLocation))]
internal class ReturnAutoCardsWithRetainToHandPatch
{
    [HarmonyPrefix]
    private static void ModifyCardPlayResultLocationPrefix(CardModel card, ref CardLocation cardLocation)
    {
        if (card.CombatState == null) throw new InvalidOperationException("CombatState is null");
        if (AutoTracker.IsBeingAutoPlayedByAuto[card] && card.IsAutoThisTurn() &&
            (card.ShouldRetainThisTurn || !Hook.ShouldFlush(card.CombatState, card.Owner)) &&
            cardLocation.pileType == PileType.Discard)
        {
            cardLocation.pileType = PileType.Hand;
            cardLocation.position = CardPilePosition.Top;
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
internal class IncludeRetainTooltipWithAutoTooltipPatch
{
    [HarmonyPostfix]
    private static void HoverTipsGetterPostfix(ref IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        // Single turn Auto doesn't automatically apply the tooltip like the keyword does, so manually do it here.
        if (AutoTracker.HasSingleTurnAuto[__instance] && !__instance.Keywords.Contains(PotentiaKeywords.Auto))
            __result = __result.Append(HoverTipFactory.FromKeyword(PotentiaKeywords.Auto));

        // Same trick as including Transform tooltop with the Prismatic tooltip. Doing this independently in separate
        // locations might be inefficient, may want to look at combining these sometime.
        if (__instance.IsAutoThisTurn() && !__result.Contains(HoverTipFactory.FromKeyword(CardKeyword.Retain)))
        {
            __result = __result.Append(HoverTipFactory.FromKeyword(CardKeyword.Retain));
        }
    }
}

[HarmonyPatch]
internal class SingleTurnAutoDescriptionPatch
{
    private static MethodBase TargetMethod()
    {
        var method =
            typeof(CardModel).GetMethod("GetDescriptionForPile", BindingFlags.NonPublic | BindingFlags.Instance);
        return method ?? throw new MissingMethodException("Could not find GetDescriptionForPile method");
    }

    [HarmonyPostfix]
    private static void GetDescriptionForPilePostfix(ref string __result, CardModel __instance)
    {
        // Single turn Auto doesn't automatically add itself to the description like the keyword does, so manually do it
        // here. Directly modifying the string like this is a rather inefficient way to do it and doesn't guarantee the
        // title will be placed in the same place that the keyword would place it, but it's a simple way that avoids
        // needing to use a Transpiler. I also don't think it's worth trying to make an elegant solution myself, as
        // generic single-turn keywords sounds like something that is likely to be supported by the base game or modding
        // libraries soon.
        if (AutoTracker.HasSingleTurnAuto[__instance] && !__instance.Keywords.Contains(PotentiaKeywords.Auto))
            __result = $"{PotentiaKeywords.Auto.GetCardText()}\n{__result}";
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.EndOfTurnCleanup))]
internal class RemoveSingleTurnAutoPatch
{
    [HarmonyPrefix]
    private static void EndOfTurnCleanupPrefix(CardModel __instance)
    {
        AutoTracker.HasSingleTurnAuto[__instance] = false;
    }
}

public static class CardModelExtensions
{
    public static bool IsAutoThisTurn(this CardModel card) =>
        card.Keywords.Contains(PotentiaKeywords.Auto) || AutoTracker.HasSingleTurnAuto[card];
}

public static class AutoTracker
{
    public static readonly SpireField<CardModel, bool> HasSingleTurnAuto = new(() => false);
    public static readonly SpireField<CardModel, bool> IsBeingAutoPlayedByAuto = new(() => false);
}