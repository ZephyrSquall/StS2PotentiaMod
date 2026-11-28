using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class CrackleCrackle() : PotentiaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShockPower>(2), new CalculationBaseVar(0), new CalculationExtraVar(1),
        new CalculatedVar("CalculatedShockApplications").WithMultiplier((_, target) =>
            target?.Powers.Count(p => p.TypeForCurrentAmount == PowerType.Debuff && !(p is ITemporaryPower)) ?? 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        int calculatedShockApplications =
            (int)((CalculatedVar)DynamicVars["CalculatedShockApplications"]).Calculate(play.Target);
        for (int i = 0; i < calculatedShockApplications; i++)
            await PowerCmd.Apply<ShockPower>(choiceContext, play.Target, DynamicVars.Power<ShockPower>().BaseValue,
                Owner.Creature, this);
    }

    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}