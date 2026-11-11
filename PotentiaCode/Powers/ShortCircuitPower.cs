using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Potentia.PotentiaCode.Powers;

public class ShortCircuitPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Owner.HasPower<ShockPower>() ? [] : [HoverTipFactory.FromPower<ShockPower>()];
}