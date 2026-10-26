using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Powers;

public class ConservePower() : PotentiaPower, IModifyRecharge
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // This is a constant to ensure the value is synced with the Conserve card.
    public const int Recharge = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Recharge", Recharge)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [PotentiaHoverTips.Recharge()];

    public int ModifyRecharge(CardModel card, int energySpent, int recharge)
    {
        if (card.Owner.Creature == Owner && card.EnergyCost.CostsX) return recharge + DynamicVars["Recharge"].IntValue;
        return recharge;
    }

    // Recharge is applied as energy is spent. This happens before BeforeCardPlayed is called, so this hook still
    // happens after this power modifies Recharge.
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.EnergyCost.CostsX) await PowerCmd.Decrement(this);
    }
}