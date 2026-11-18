using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class Renew() : PotentiaCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        if (!IsUpgraded) xValue = Math.Max(0, xValue - 1);
        await PowerCmd.Apply<RenewPower>(choiceContext, Owner.Creature, xValue, Owner.Creature, this);
    }
}