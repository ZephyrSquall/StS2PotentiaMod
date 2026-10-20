using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Hooks;

public class PotentiaHooks
{
    public static int ModifyRecharge(CardModel card, int? energySpent)
    {
        // energySpent indicates the amount of energy spent on the card. If it is null, that means the card hasn't
        // actually been played, and we're calling ModifyRecharge to get a number for display purposes; in this case,
        // predict what the energy spent will be.
        int actualOrPredictedEnergySpent = energySpent ?? card.EnergyCost.GetAmountToSpend();
        int recharge = RechargeTracker.RechargeAmount[card];

        foreach (var model in card.CombatState.IterateHookListeners().OfType<IModifyRecharge>())
            recharge = model.ModifyRecharge(card, actualOrPredictedEnergySpent, recharge);
        return recharge;
    }
}