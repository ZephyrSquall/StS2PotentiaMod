using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

public class PotentiaHooks
{
    public static int ModifyRecharge(CardModel card, int originalRecharge)
    {
        int recharge = originalRecharge;
        foreach (var model in card.CombatState.IterateHookListeners().OfType<IModifyRecharge>())
            recharge = model.ModifyRecharge(card, recharge);
        return recharge;
    }
}