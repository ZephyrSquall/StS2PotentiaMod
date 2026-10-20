using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
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
            if (cardModel.Keywords.Contains(PotentiaKeywords.Auto))
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
        if (AutoTracker.IsBeingAutoPlayedByAuto[card] && card.Keywords.Contains(PotentiaKeywords.Auto) &&
            card.Keywords.Contains(CardKeyword.Retain) && cardLocation.pileType == PileType.Discard)
        {
            cardLocation.pileType = PileType.Hand;
            cardLocation.position = CardPilePosition.Top;
        }
    }
}

public class AutoTracker
{
    public static readonly SpireField<CardModel, bool> IsBeingAutoPlayedByAuto = new(() => false);
}