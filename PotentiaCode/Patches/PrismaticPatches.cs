using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Hooks;
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