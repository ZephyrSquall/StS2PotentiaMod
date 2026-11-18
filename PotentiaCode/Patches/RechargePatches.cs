using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.TextEffects;
using Potentia.PotentiaCode.CombatHistoryEntries;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Patches;

[HarmonyPatch]
internal class RechargeDescriptionPatch
{
    private static MethodBase TargetMethod()
    {
        var method =
            typeof(CardModel).GetMethod("GetDescriptionForPile", BindingFlags.NonPublic | BindingFlags.Instance);
        return method ?? throw new MissingMethodException("Could not find GetDescriptionForPile method");
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GetDescriptionForPileTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Add modifier text immediately before where added "Replay" text would go if the card also had Replay.
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel)!.GetEnchantedReplayCount()))
            .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
            // Load the "source" local variable onto the stack, which is index 5 in the raw IL.
            // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
            // to load that again.
            .InsertAndAdvance(CodeInstruction.LoadLocal(5))
            // Load the "previewType" argument onto the stack, which is index 2 in the raw IL (note that since this is
            // an instance method, argument 0 is "this", so the rest of the arguments are shifted down one index).
            .InsertAndAdvance(CodeInstruction.LoadArgument(2))
            // Load the "target" argument onto the stack, which is index 3 in the raw IL.
            .InsertAndAdvance(CodeInstruction.LoadArgument(3))
            .InsertAndAdvance(CodeInstruction.Call(() => TranspilerAddedCode(default!, default!, default, default)))
            // Store the top of the stack into the "source" local variable.
            .InsertAndAdvance(CodeInstruction.StoreLocal(5))
            // Load the cardModel onto the stack again to restore its original state before this patch.
            .InsertAndAdvance(CodeInstruction.LoadArgument(0));

        return codeMatcher.Instructions();
    }

    public static List<string> TranspilerAddedCode(CardModel cardModel, List<string> source,
        CardModel.DescriptionPreviewType previewType, Creature? target)
    {
        int recharge = PotentiaQueries.GetRecharge(cardModel, target);
        if (recharge > 0)
        {
            string formattedRechargeValue;
            if ((previewType == CardModel.DescriptionPreviewType.Upgrade || cardModel.UpgradePreviewType.IsPreview()) &&
                RechargeTracker.IsRechargeIncreasedOnUpgrade[cardModel])
                formattedRechargeValue = $"[green]{recharge}[/green]";
            else
            {
                int comparison = recharge - (RechargeTracker.RechargeAmount[cardModel] +
                                             RechargeTracker.RechargeAmountUntilPlayed[cardModel]);
                formattedRechargeValue = StsTextUtilities.HighlightChangeText(recharge.ToString(), comparison);
            }

            LocString locString;
            if (cardModel.IsInCombat || cardModel.UpgradePreviewType == CardUpgradePreviewType.Combat)
            {
                locString = new LocString("static_hover_tips", "POTENTIA-RECHARGE.extraTextInCombat");
                locString.Add("Recharge", formattedRechargeValue);
                // Intentional integer division to ensure fractional amounts are rounded down, as is always supposed to
                // be the case in Slay the Spire 2.
                locString.Add("Energy", cardModel.EnergyCost.GetAmountToSpend() * recharge / 3);
            }
            else
            {
                locString = new LocString("static_hover_tips", "POTENTIA-RECHARGE.extraText");
                locString.Add("Recharge", formattedRechargeValue);
            }

            source.Add(locString.GetFormattedText());
        }

        return source;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
public class RechargeHoverTipPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> HoverTipsGetterTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Add modifier hover tip immediately before "Replay" hover tip.
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel)!.GetEnchantedReplayCount()))
            .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
            // Load the "list" local variable onto the stack, which is index 0 in the raw IL.
            // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
            // to load that again.
            .InsertAndAdvance(CodeInstruction.LoadLocal(0))
            .InsertAndAdvance(CodeInstruction.Call(() => TranspilerAddedCode(default!, default!)))
            // Store the top of the stack into the "list" local variable.
            .InsertAndAdvance(CodeInstruction.StoreLocal(0))
            // Load the cardModel onto the stack again to restore its original state before this patch.
            .InsertAndAdvance(CodeInstruction.LoadArgument(0));

        return codeMatcher.Instructions();
    }

    public static List<IHoverTip> TranspilerAddedCode(CardModel cardModel, List<IHoverTip> list)
    {
        int recharge = PotentiaQueries.GetRecharge(cardModel);
        if (recharge > 0)
        {
            LocString title = new LocString("static_hover_tips", "POTENTIA-RECHARGE_DYNAMIC.title");
            LocString description = new LocString("static_hover_tips", "POTENTIA-RECHARGE_DYNAMIC.description");
            description.Add("Recharge", recharge);
            list.Add(new HoverTip(title, description));
        }

        return list;
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
internal class RegainEnergyPatch
{
    [HarmonyPostfix]
    private static void AfterCardPlayedPostfix(ref Task __result, ICombatState combatState, CardPlay cardPlay)
    {
        __result = AsyncWrapper(__result, combatState, cardPlay);
    }

    private static async Task AsyncWrapper(Task originalTask, ICombatState combatState, CardPlay cardPlay)
    {
        int recharge = PotentiaQueries.GetRecharge(cardPlay.Card, cardPlay.Target, cardPlay.Resources.EnergySpent,
            out List<IAfterModifyingRecharge> modifiers);

        if (recharge > 0)
        {
            // Intentional integer division to ensure fractional amounts are rounded down, as is always supposed to be
            // the case in Slay the Spire 2.
            int energy = (cardPlay.Resources.EnergySpent * recharge) / 3;
            await PlayerCmd.GainEnergy(energy, cardPlay.Card.Owner);
            CombatManager.Instance.History.Recharged(combatState, recharge, energy, cardPlay.Card.Owner);
            await PotentiaHooks.AfterRecharged(cardPlay.Card, recharge, energy);
        }

        foreach (IAfterModifyingRecharge modifier in modifiers)
        {
            await modifier.AfterModifyingRecharge(cardPlay.Card);
        }

        await originalTask;
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.AfterCardPlayedCleanup))]
internal class ClearTemporaryRechargeAfterCardPlayed
{
    [HarmonyPrefix]
    private static void AfterCardPlayedCleanupPrefix(CardModel ____card)
    {
        RechargeTracker.RechargeAmountUntilPlayed[____card] = 0;
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.MutableClone))]
public class RechargeClonePatch
{
    [HarmonyPostfix]
    private static void MutableClonePostfix(ref AbstractModel __result, AbstractModel __instance)
    {
        if (__result is CardModel resultCardModel && __instance is CardModel instanceCardModel)
        {
            RechargeTracker.RechargeAmount[resultCardModel] = RechargeTracker.RechargeAmount[instanceCardModel];
            RechargeTracker.RechargeAmountUntilPlayed[resultCardModel] =
                RechargeTracker.RechargeAmountUntilPlayed[instanceCardModel];
            RechargeTracker.IsRechargeIncreasedOnUpgrade[resultCardModel] =
                RechargeTracker.IsRechargeIncreasedOnUpgrade[instanceCardModel];
        }
    }
}

public static class RechargeTracker
{
    public static readonly SpireField<CardModel, int> RechargeAmount = new(() => 0);
    public static readonly SpireField<CardModel, int> RechargeAmountUntilPlayed = new(() => 0);
    public static readonly SpireField<CardModel, bool> IsRechargeIncreasedOnUpgrade = new(() => false);
}