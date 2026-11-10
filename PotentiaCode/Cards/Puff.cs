using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Potentia.PotentiaCode.Cards;

public class Puff() : PotentiaCard(0, CardType.Power, CardRarity.Rare, TargetType.AnyAlly)
{
    protected override bool HasEnergyCostX => true;

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2), new PowerVar<DexterityPower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<DexterityPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var xValue = ResolveEnergyXValue();
        for (int i = 0; i < xValue; i++)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, play.Target,
                DynamicVars.Power<StrengthPower>().BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<DexterityPower>(choiceContext, play.Target,
                DynamicVars.Power<DexterityPower>().BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1);
        DynamicVars.Power<DexterityPower>().UpgradeValueBy(1);
    }
}