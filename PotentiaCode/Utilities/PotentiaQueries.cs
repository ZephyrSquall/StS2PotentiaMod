using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Utilities;

public class PotentiaQueries
{
    public static int GetRecharge(CardModel card, Creature? target = null, int? energySpent = null,
        List<IAfterModifyingRecharge>? modifiers = null)
    {
        if (card.IsInCombat || card.UpgradePreviewType == CardUpgradePreviewType.Combat)
        {
            return PotentiaHooks.ModifyRecharge(card, target, energySpent, modifiers);
        }

        modifiers = [];
        return RechargeTracker.RechargeAmount[card];
    }
}