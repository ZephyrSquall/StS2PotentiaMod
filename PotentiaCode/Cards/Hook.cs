using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class Hook() : PotentiaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move), new CalculationBaseVar(0), new CalculationExtraVar(1),
        new CalculatedVar("CalculatedCards").WithMultiplier((_, target) =>
            target != null
                ? target.Powers.Count(p => p.TypeForCurrentAmount == PowerType.Debuff && p is not ITemporaryPower)
                : 0)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        // Determine cards to draw before damage is dealt. This prevents two issues:
        //  - If the damage inflicted a new unique debuff (e.g. because the player has Reaper Form and the enemy doesn't
        //      already have Doom), this would cause the actual card draw to differ from the previewed card draw.
        //  - If the damage kills the enemy, they get removed from combat, and hence Hook would no longer be able to see
        //      the enemy's debuffs. This would cause Hook to draw zero cards, no matter how many debuffs the enemy had.
        decimal cardsToDraw = ((CalculatedVar)DynamicVars["CalculatedCards"]).Calculate(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, cardsToDraw, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}