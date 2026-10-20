using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Utilities;

public class PotentiaQueries
{
    public static int GetRecharge(CardModel card)
    {
        if (card.IsInCombat)
        {
            return PotentiaHooks.ModifyRecharge(card, RechargeTracker.RechargeAmount[card]);
        }

        return RechargeTracker.RechargeAmount[card];
    }
}