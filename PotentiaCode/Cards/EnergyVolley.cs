using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

[Pool(typeof(TokenCardPool))]
public class EnergyVolley : PotentiaCard
{
    public EnergyVolley() : base(0, CardType.Attack, CardRarity.Token, TargetType.RandomEnemy)
    {
        RechargeTracker.RechargeAmount[this] = 3;
    }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null) throw new InvalidOperationException("CombatState is null");
        var xValue = ResolveEnergyXValue();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(xValue)
            .FromCard(this, play)
            .TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}