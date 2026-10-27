using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class Renew() : PotentiaCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        for (int i = 0; i < xValue; i++) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<RenewPower>(choiceContext, Owner.Creature, xValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(1);
}