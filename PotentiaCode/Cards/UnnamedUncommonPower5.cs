using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class UnnamedUncommonPower5() : PotentiaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<UnnamedUncommonPower5Power>(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Transform), HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<UnnamedUncommonPower5Power>(choiceContext, Owner.Creature,
            DynamicVars.Power<UnnamedUncommonPower5Power>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<UnnamedUncommonPower5Power>().UpgradeValueBy(1);
}