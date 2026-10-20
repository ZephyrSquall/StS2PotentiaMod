using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]
internal class TransformAfterPrismaticCardDrawnPatch
{
    [HarmonyPostfix]
    private static void AfterCardDrawnPostfix(ref Task __result, CardModel card)
    {
        __result = AsyncWrapper(__result, card);
    }

    private static async Task AsyncWrapper(Task originalTask, CardModel card)
    {
        await originalTask;

        if (card.Keywords.Contains(PotentiaKeywords.Prismatic))
        {
            var cardPileAddResult = await CardCmd.TransformToRandom(card, card.Owner.RunState.Rng.CombatCardSelection);
            if (cardPileAddResult.success)
            {
                cardPileAddResult.cardAdded.AddKeyword(PotentiaKeywords.Prismatic);
            }
        }
    }
}

// This would probably be more efficient as a Transpiler that injects the check inside the same foreach loop that checks
// for Ethereal and adds the Exhaust tooltip, but a Transpiler is more likely to break if other mods patch this method
// or a Slay the Spire 2 update changes this method, so for now I'm opting to use a simpler Postfix.
[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
internal class IncludeTransformTooltipWithPrismaticTooltipPatch
{
    [HarmonyPostfix]
    private static void HoverTipsGetterPostfix(ref IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        if (__instance.Keywords.Contains(PotentiaKeywords.Prismatic) &&
            !__result.Contains(HoverTipFactory.Static(StaticHoverTip.Transform)))
        {
            __result = __result.Append(HoverTipFactory.Static(StaticHoverTip.Transform));
        }
    }
}