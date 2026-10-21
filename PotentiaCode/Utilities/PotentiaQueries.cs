using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Utilities;

public class PotentiaQueries
{
    public static int GetRecharge(CardModel card, int? energySpent = null)
    {
        if (card.IsInCombat || card.UpgradePreviewType == CardUpgradePreviewType.Combat)
        {
            return PotentiaHooks.ModifyRecharge(card, energySpent);
        }

        return RechargeTracker.RechargeAmount[card];
    }
}