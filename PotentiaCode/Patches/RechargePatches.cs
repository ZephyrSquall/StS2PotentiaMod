using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.TextEffects;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Patches;

[HarmonyPatch]
internal class RechargeDescriptionPatch
{
    private static MethodBase TargetMethod()
    {
        var method =
            typeof(CardModel).GetMethod("GetDescriptionForPile", BindingFlags.NonPublic | BindingFlags.Instance);
        return method;
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GetDescriptionForPileTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        // Add modifier text immediately before where added "Replay" text would go if the card also had Replay.
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
            .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
            // Load the "source" local variable onto the stack, which is index 5 in the raw IL.
            // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
            // to load that again.
            .InsertAndAdvance(CodeInstruction.LoadLocal(5))
            .InsertAndAdvance(CodeInstruction.Call(() => TranspilerAddedCode(default, default)))
            // Store the top of the stack into the "source" local variable.
            .InsertAndAdvance(CodeInstruction.StoreLocal(5))
            // Load the cardModel onto the stack again to restore its original state before this patch.
            .InsertAndAdvance(CodeInstruction.LoadArgument(0));

        return codeMatcher.Instructions();
    }

    public static List<string> TranspilerAddedCode(CardModel cardModel, List<string> source)
    {
        int recharge = PotentiaQueries.GetRecharge(cardModel);
        if (recharge > 0)
        {
            LocString locString = new LocString("static_hover_tips", "POTENTIA-RECHARGE.extraText");
            string rechargeText = StsTextUtilities.HighlightChangeText(recharge.ToString(),
                recharge - RechargeTracker.RechargeAmount[cardModel]);
            locString.Add("Recharge", rechargeText);
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
        codeMatcher.MatchStartForward(CodeMatch.Calls(() => default(CardModel).GetEnchantedReplayCount()))
            .ThrowIfInvalid("Could not find call to GetEnchantedReplayCount")
            // Load the "list" local variable onto the stack, which is index 0 in the raw IL.
            // Note the cardModel happens to already be the last thing on the stack before this instruction, so no need
            // to load that again.
            .InsertAndAdvance(CodeInstruction.LoadLocal(0))
            .InsertAndAdvance(CodeInstruction.Call(() => TranspilerAddedCode(default, default)))
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

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterEnergySpent))]
internal class RegainEnergyPatch
{
    [HarmonyPostfix]
    private static void AfterEnergySpentPostfix(ref Task __result, CardModel card, int amount)
    {
        __result = AsyncWrapper(__result, card, amount);
    }

    private static async Task AsyncWrapper(Task originalTask, CardModel card, int amount)
    {
        await originalTask;

        int recharge = PotentiaQueries.GetRecharge(card, amount);
        if (recharge > 0)
        {
            // Intentional integer division to ensure fractional amounts are rounded down, as is always supposed to be
            // the case in Slay the Spire 2.
            await PlayerCmd.GainEnergy((amount * recharge) / 3, card.Owner);
        }
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
        }
    }
}

public class RechargeTracker
{
    public static readonly SpireField<CardModel, int> RechargeAmount = new(() => 0);
}