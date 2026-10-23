using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

public class UnnamedAncientSkill1 : PotentiaCard
{
    public UnnamedAncientSkill1() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        RechargeTracker.RechargeAmount[this] = 2;
    }

    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => EnergyCost.GetAmountToSpend() <= DynamicVars.Energy.IntValue;

    public override TargetType TargetType =>
        IsInCombat && EnergyCost.GetAmountToSpend() <= DynamicVars.Energy.IntValue
            ? TargetType.AllEnemies
            : TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move), new EnergyVar(2),
        new BlockVar(9, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();

        if (play.Target == null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithValueProp(DynamicVars.Damage.Props)
                .WithHitCount(xValue)
                .FromCard(this, play)
                .TargetingAllOpponents(CombatState)
                .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
                .Execute(choiceContext);
        else
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithValueProp(DynamicVars.Damage.Props)
                .WithHitCount(xValue)
                .FromCard(this, play)
                .Targeting(play.Target)
                .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
                .Execute(choiceContext);

        if (play.Resources.EnergySpent <= DynamicVars.Energy.IntValue)
            for (int i = 0; i < xValue; i++)
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}