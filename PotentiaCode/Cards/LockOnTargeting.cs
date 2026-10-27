using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
        Creature? target = null;

        for (int i = 0; i < xValue; i++)
        {
            target = GetNextTarget(CombatState, target);
            if (target == null) break;
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
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

    // Gets the next target. Enemies are ordered by the global X position of their corresponding Creature node (CombatId
    // is not sequential with their order on-screen if an enemy summons more enemies mid-combat).
    //
    // The next target is the enemy with the largest X position that is smaller than the last target's X position. If
    // there is no last target (it is null) or there is no enemy with a smaller X position, then it gets the enemy with
    // the largest X position instead, resetting the loop. Note that CombatState.HittableEnemies needs to be checked
    // each time, as enemies dying to earlier hits will change the available targets.
    //
    // If there are no possible targets (I believe this can only happen when Test Subject is changing phases), returns
    // null.
    private Creature? GetNextTarget(ICombatState combatState, Creature? lastTarget)
    {
        float lastXPos = lastTarget?.GetCreatureNode()?.GlobalPosition.X ?? float.MaxValue;
        IEnumerable<Creature> hittableTargets = combatState.HittableEnemies.Where(t => t.GetCreatureNode() != null);
        IEnumerable<Creature> validTargets =
            hittableTargets.Where(t => t.GetCreatureNode()?.GlobalPosition.X < lastXPos);
        if (!validTargets.Any()) validTargets = hittableTargets;

        Creature? nextTarget = validTargets.MaxBy(t => t.GetCreatureNode().GlobalPosition.X);
        return nextTarget;
    }
}