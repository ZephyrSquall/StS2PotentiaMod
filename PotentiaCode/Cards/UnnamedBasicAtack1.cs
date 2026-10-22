using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class UnnamedBasicAttack1() : PotentiaCard(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => EnergyCost.GetAmountToSpend() == DynamicVars.Energy.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move), new EnergyVar(1),
        new BlockVar(8, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithValueProp(DynamicVars.Damage.Props)
            .WithHitCount(xValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithAttackerAnim("Cast", this.Owner.Character.CastAnimDelay)
            .Execute(choiceContext);
        if (play.Resources.EnergySpent == DynamicVars.Energy.IntValue)
            CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(3);
    }
}