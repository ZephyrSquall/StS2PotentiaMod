using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Cards;

namespace Potentia.PotentiaCode.Powers;

public class DiodePower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new("AdditionalDamage", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<ShockPower>()
    ];

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource?.Owner.Creature == Owner && !cardSource.EnergyCost.CostsX ? Amount : 0;
    }

    public override Decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, Decimal amount,
        Creature? target, CardModel? cardSource)
    {
        // Beam Shield's preview value does not work correctly with this power due to using a CalculatedVar instead of a
        // PowerVar to store its value. As a hacky workaround to force Beam Shield to work, it is specifically excluded
        // here, and instead Beam Shield itself will explicitly check if its owner has the Diode power and handle
        // increasing its amount itself if necessary. I don't think there is a better way of handling this, as the
        // basegame currently only uses this ModifyPowerAmountGivenAdditive hook for Snecko Skull and there are no
        // Poison sources using a CalculatedVar that the basegame has to worry about.
        return power is ShockPower && giver == Owner && cardSource?.Owner.Creature == Owner &&
               !cardSource.EnergyCost.CostsX && cardSource is not BeamShield
            ? Amount
            : 0;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return cardSource?.Owner.Creature == Owner && !cardSource.EnergyCost.CostsX
            ? DynamicVars["AdditionalDamage"].BaseValue
            : 0;
    }

    public void IncreaseAdditionalDamage(int amount)
    {
        DynamicVars["AdditionalDamage"].BaseValue += amount;
    }
}