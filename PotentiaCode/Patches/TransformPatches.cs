using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using Potentia.PotentiaCode.CombatHistoryEntries;
using Potentia.PotentiaCode.Hooks;

namespace Potentia.PotentiaCode.Patches;

[HarmonyPatch]
internal class AddTransformHookPatch
{
    // There are multiple overloads of Transform, which all ultimately call through to one particular Transform overload
    // that handles every possible overload. Get that particular handle-everything overload.
    private static readonly MethodInfo transformOverload = AccessTools.Method(typeof(CardCmd),
        nameof(CardCmd.Transform), [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)]);

    private static MethodBase TargetMethod()
    {
        return AccessTools.AsyncMoveNext(transformOverload);
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TransformTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo combinedMethod = AccessTools.Method(typeof(AddTransformHookPatch), nameof(TranspilerAddedCode));

        var codeMatcher = new CodeMatcher(instructions);

        // To get the game to run our added method asynchronously, bundle it together with an already-existing async
        // method call, in this case Hook.AfterCardGeneratedForCombat. This is pretty much exactly the hook we want
        // except we want the AfterTransform hook to run exclusively here and not for any other way of creating cards.
        codeMatcher
            .MatchStartForward(CodeMatch.Calls(() => Hook.AfterCardGeneratedForCombat(default, default, default)))
            .ThrowIfInvalid("Could not find call to Hook.AfterCardGeneratedForCombat")
            .SetInstruction(new CodeInstruction(OpCodes.Call, combinedMethod));

        return codeMatcher.Instructions();
    }

    // This intentionally has the same arguments as Hook.AfterCardGeneratedForCombat so this can directly replace it.
    public static async Task TranspilerAddedCode(ICombatState combatState, CardModel cardAdded, Player owner)
    {
        CombatHistory combatHistory = CombatManager.Instance.History;
        combatHistory.Add(combatState,
            new CardTransformedEntry(cardAdded, owner, combatState.RoundNumber, combatState.CurrentSide, combatHistory,
                combatState.Players));

        await PotentiaHooks.AfterTransform(combatState, cardAdded, owner);

        // Add the original hook back so the game doesn't skip it.
        await Hook.AfterCardGeneratedForCombat(combatState, cardAdded, owner);
    }
}