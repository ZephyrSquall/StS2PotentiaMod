using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class StaticDischarge() : PotentiaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(StaticDischargePower.EnergyThreshold), new PowerVar<StaticDischargePower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StaticDischargePower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StaticDischargePower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<StaticDischargePower>().UpgradeValueBy(1);
}