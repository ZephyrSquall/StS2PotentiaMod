using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Hooks;

public class PotentiaHooks
{
    public static int ModifyRecharge(CardModel card, int? energySpent)
    {
        int recharge = RechargeTracker.RechargeAmount[card];

        // In my testing, if the player is prompted for a choice in the middle of combat (e.g. choosing a card to
        // discard with Survivor, or a card to upgrade with Armaments), and then quits to the title screen before
        // finishing the choice, then ModifyRecharge gets called with a card whose CombatState is null. Without this
        // null check to terminate ModifyRecharge early, the game would crash. I'm unsure exactly why this specific
        // scenario causes this to happen, as ordinarily ModifyRecharge isn't called at all when quitting the game; I'm
        // assuming this is caused by a bug in the base game as the terminal spits out a lot of errors about a parent
        // node being too busy to move child nodes and suggesting the developers used `move_child()` when they should
        // have used `move_child.call_deferred(child, index)`.
        if (card.CombatState == null) return recharge;

        // energySpent indicates the amount of energy spent on the card. If it is null, that means the card hasn't
        // actually been played, and we're calling ModifyRecharge to get a number for display purposes; in this case,
        // predict what the energy spent will be.
        int actualOrPredictedEnergySpent = energySpent ?? card.EnergyCost.GetAmountToSpend();

        foreach (var model in card.CombatState.IterateHookListeners().OfType<IModifyRecharge>())
            recharge = model.ModifyRecharge(card, actualOrPredictedEnergySpent, recharge);
        return recharge;
    }
}