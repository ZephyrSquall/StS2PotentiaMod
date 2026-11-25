using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Powers;

public class AcidRainPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner.Player != null && participants.Contains(Owner))
        {
            int cardCount = PileType.Hand.GetPile(Owner.Player).Cards.Count;
            if (cardCount > 0)
            {
                if (Owner.CombatState == null) throw new InvalidOperationException("Owner.CombatState is null");
                Creature? target = Owner.Player.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
                if (target != null)
                {
                    Flash();
                    await CreatureCmd.Damage(choiceContext, target, cardCount * Amount, ValueProp.Unpowered, Owner);
                }
            }
        }
    }
}