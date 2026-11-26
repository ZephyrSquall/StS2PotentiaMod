using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class Diode() : PotentiaCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block), HoverTipFactory.FromPower<ShockPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        if (IsUpgraded) xValue *= 2;
        await PowerCmd.Apply<DiodePower>(choiceContext, Owner.Creature, xValue, Owner.Creature, this);
    }
}