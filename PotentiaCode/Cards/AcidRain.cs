using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class AcidRain() : PotentiaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AcidRainPower>(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<AcidRainPower>(choiceContext, Owner.Creature, DynamicVars.Power<AcidRainPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<AcidRainPower>().UpgradeValueBy(1);
}