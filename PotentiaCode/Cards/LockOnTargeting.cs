using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class LockOnTargeting() : PotentiaCard(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override bool HasEnergyCostX => true;

    protected override bool ShouldGlowGoldInternal => EnergyCost.GetAmountToSpend() == DynamicVars.Energy.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        if (CombatState == null) throw new InvalidOperationException("CombatState is null");
        List<Creature> targets = GetRightmostTargets(CombatState, xValue);
        // We must use the fully qualified name here to disambiguate from Potentia's Hook card.
        int hitCount =
            (int)MegaCrit.Sts2.Core.Hooks.Hook.ModifyAttackHitCount(CombatState,
                DamageCmd.Attack(0).FromCard(this, play), xValue);

        // We cannot use a DamageCmd like we would usually do for Attacks because a DamageCmd only supports hitting a
        // single enemey, a random enemy, or all enemies. Using an AttackContext directly gets around this issue because
        // we can specify the exact list of targets to be damaged.
        await using (AttackContext attackContext =
                     await AttackCommand.CreateContextAsync(CombatState, choiceContext, play))
        {
            for (int i = 0; i < hitCount && Owner.Creature.IsAlive; i++)
            {
                // Targets might become unhittable during the attack if an earlier hit kills them. We must handle this
                // or else the game will crash when trying to hit them again.
                targets.RemoveAll(t => !t.IsHittable);
                if (targets.Count == 0) break;
                await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
                IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, targets,
                    DynamicVars.Damage, Owner.Creature, this, play);
                attackContext.AddHit(damageResults);
            }
        }
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource == this && props.IsPoweredAttack() &&
            ((cardPlay == null && EnergyCost.GetAmountToSpend() == DynamicVars.Energy.IntValue) ||
             (cardPlay != null && cardPlay.Resources.EnergySpent == DynamicVars.Energy.IntValue)))
            return 2;
        return 1;
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);

    private List<Creature> GetRightmostTargets(ICombatState combatState, int numberOfTargets)
    {
        return combatState.HittableEnemies
            .OrderByDescending(c => c.GetCreatureNode()?.GlobalPosition.X ?? float.MaxValue)
            .Take(numberOfTargets)
            .ToList();
    }
}