using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Potentia.PotentiaCode.Powers;

public class ShockingGraspPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        Owner.HasPower<ShockPower>() ? [] : [HoverTipFactory.FromPower<ShockPower>()];

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner) && !Owner.IsDead) await PowerCmd.Decrement(this);
    }
}