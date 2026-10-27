using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Potentia.PotentiaCode.Powers;

public class RenewPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        return player == Owner.Player ? amount + Amount : amount;
    }
}