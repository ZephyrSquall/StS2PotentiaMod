using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

public class GalvanicToughness : PotentiaCard
{
    public GalvanicToughness() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        RechargeTracker.IsRechargeIncreasedOnUpgrade[this] = true;
    }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        await PowerCmd.Apply<BlurPower>(choiceContext, Owner.Creature, xValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => RechargeTracker.RechargeAmount[this] += 1;
    protected override void AfterDowngraded() => RechargeTracker.RechargeAmount[this] -= 1;
}