using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class Diode() : PotentiaCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DiodePower>(1), new("AdditionalDamage", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<ShockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();

        for (int i = 0; i < xValue; i++)
        {
            (await PowerCmd.Apply<DiodePower>(choiceContext, Owner.Creature, DynamicVars.Power<DiodePower>().BaseValue,
                Owner.Creature, this))?.IncreaseAdditionalDamage(DynamicVars["AdditionalDamage"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<DiodePower>().UpgradeValueBy(1);
        DynamicVars["AdditionalDamage"].UpgradeValueBy(1);
    }
}