using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Potentia.PotentiaCode.Patches;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class SpotWeakness : PotentiaCard
{
    public SpotWeakness() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        RechargeTracker.RechargeAmount[this] = 1;
    }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SpotWeaknessPower>(3), new PowerVar<WeakPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        int xValue = ResolveEnergyXValue();

        for (int i = 0; i < xValue; i++)
            await PowerCmd.Apply<SpotWeaknessPower>(choiceContext, play.Target,
                DynamicVars.Power<SpotWeaknessPower>().BaseValue, Owner.Creature, this);
        for (int i = 0; i < xValue; i++)
            await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars.Power<WeakPower>().BaseValue,
                Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<SpotWeaknessPower>().UpgradeValueBy(1);
}