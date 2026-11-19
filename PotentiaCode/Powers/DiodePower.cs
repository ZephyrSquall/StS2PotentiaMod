using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Powers;

public class DiodePower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new("AdditionalDamage", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return cardSource != null && cardSource.Owner.Creature == Owner && !cardSource.EnergyCost.CostsX ? Amount : 0;
    }

    public override Decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, Decimal amount,
        Creature? target, CardModel? cardSource)
    {
        return power.GetTypeForAmount(amount) == PowerType.Debuff && cardSource != null &&
               cardSource.Owner.Creature == Owner && !cardSource.EnergyCost.CostsX
            ? Amount * Math.Sign(amount)
            : 0;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return cardSource != null && cardSource.Owner.Creature == Owner && !cardSource.EnergyCost.CostsX
            ? DynamicVars["AdditionalDamage"].BaseValue
            : 0;
    }

    public void IncreaseAdditionalDamage(int amount)
    {
        DynamicVars["AdditionalDamage"].BaseValue += amount;
    }
}