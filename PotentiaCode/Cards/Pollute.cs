using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class Pollute() : PotentiaCard(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DemisePower>(2), new PowerVar<ShockPower>(2), new PowerVar<StranglePower>(2), new RepeatVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DemisePower>(), HoverTipFactory.FromPower<ShockPower>(),
        HoverTipFactory.FromPower<StranglePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        
        IReadOnlyList<(PowerModel power, decimal baseValue)> debuffOptionsWithBaseValues =
        [
            (ModelDb.Power<DemisePower>(), DynamicVars.Power<DemisePower>().BaseValue),
            (ModelDb.Power<ShockPower>(), DynamicVars.Power<ShockPower>().BaseValue),
            (ModelDb.Power<StranglePower>(), DynamicVars.Power<StranglePower>().BaseValue),
        ];

        for (int i = 0; i < DynamicVars.Repeat.BaseValue; i++)
        {
            (PowerModel canonicalDebuffPower, decimal baseValue) =
                Owner.RunState.Rng.Niche.NextItem(debuffOptionsWithBaseValues);
            PowerModel debuffPower = canonicalDebuffPower.ToMutable();
            await PowerCmd.Apply(choiceContext, debuffPower, play.Target, baseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Repeat.UpgradeValueBy(1);
}