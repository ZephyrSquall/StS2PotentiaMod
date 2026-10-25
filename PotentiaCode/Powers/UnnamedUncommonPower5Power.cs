using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Hooks;

namespace Potentia.PotentiaCode.Powers;

public class UnnamedUncommonPower5Power() : PotentiaPower, IAfterTransform
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Transform), HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public async Task AfterTransform(ICombatState combatState, CardModel card, Player? transformer)
    {
        if (transformer != null && transformer.Creature == Owner)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}