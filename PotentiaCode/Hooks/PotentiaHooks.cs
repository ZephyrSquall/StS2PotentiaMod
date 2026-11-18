using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Hooks;

public class PotentiaHooks
{
    public static async Task AfterCardTransformedForCombat(ICombatState combatState, CardModel card)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IAfterCardTransformedForCombat>())
            await model.AfterCardTransformedForCombat(card);
    }

    public static async Task AfterRecharged(CardModel card, int recharge, int energy)
    {
        if (card.CombatState == null) throw new InvalidOperationException("CombatState is null");
        foreach (var model in card.CombatState.IterateHookListeners().OfType<IAfterRecharged>())
            await model.AfterRecharged(card, recharge, energy);
    }

    public static int ModifyRecharge(CardModel card, Creature? target, int? energySpent,
        out List<IAfterModifyingRecharge> modifiers)
    {
        int recharge = RechargeTracker.RechargeAmount[card] + RechargeTracker.RechargeAmountUntilPlayed[card];
        modifiers = [];

        // In my testing, if the player is prompted for a choice in the middle of combat (e.g. choosing a card to
        // discard with Survivor, or a card to upgrade with Armaments), and then quits to the title screen before
        // finishing the choice, then ModifyRecharge gets called with a card whose CombatState is null. Without this
        // null check to terminate ModifyRecharge early, the game would crash. I'm unsure exactly why this specific
        // scenario causes this to happen, as ordinarily ModifyRecharge isn't called at all when quitting the game; I'm
        // assuming this is caused by a bug in the base game as the terminal spits out a lot of errors about a parent
        // node being too busy to move child nodes and suggesting the developers used `move_child()` when they should
        // have used `move_child.call_deferred(child, index)`.
        if (card.CombatState == null) return recharge;

        // energySpent indicates the amount of energy spent on the card. If it is null, that has a special meaning: This
        // card hasn't actually been played, and we're calling ModifyRecharge to get a preview number for display
        // purposes; in this case, predict what the energy spent will be.
        bool isPreview = energySpent is null;
        int actualOrPredictedEnergySpent = energySpent ?? card.EnergyCost.GetAmountToSpend();

        foreach (var model in card.CombatState.IterateHookListeners().OfType<IModifyRecharge>())
        {
            int newRecharge = model.ModifyRecharge(card, actualOrPredictedEnergySpent, recharge, target, isPreview);
            if (energySpent != null && newRecharge != recharge &&
                model is IAfterModifyingRecharge afterModifyingRechargeModel)
                modifiers.Add(afterModifyingRechargeModel);
            recharge = newRecharge;
        }

        return recharge;
    }
}