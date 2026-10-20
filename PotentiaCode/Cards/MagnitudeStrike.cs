using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

public class MagnitudeStrike : PotentiaCard
{
    public MagnitudeStrike() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        RechargeTracker.RechargeAmount[this] = 2;
    }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(xValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}