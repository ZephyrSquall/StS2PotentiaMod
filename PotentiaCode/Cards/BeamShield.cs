using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class BeamShield() : PotentiaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("BlockThreshold", 4), new CalculationBaseVar(2), new CalculationExtraVar(1),
        // Note this multiplier calculation explicitly checks for the Diode power. If we didn't do this, Beam Shield's
        // preview value would not be correct when it is boosted by Diode. See DiodePower for more details.
        new CalculatedVar("CalculatedShock").WithMultiplier((card, _) =>
        {
            decimal multiplier = Math.Floor(card.Owner.Creature.Block / card.DynamicVars["BlockThreshold"].BaseValue);
            // Normally, CalculatedVars preview their updated value everywhere, but external effects that modify a
            // value only preview that modified value when the card is in the Hand. Emulate that here.
            if (card.Pile?.Type is PileType.Hand or PileType.Play)
                multiplier += card.Owner.Creature.GetPowerAmount<DiodePower>();
            return multiplier;
        }),
        new BlockVar(8, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<ShockPower>(choiceContext, play.Target,
            ((CalculatedVar)DynamicVars["CalculatedShock"]).Calculate(play.Target), Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(1);
        DynamicVars.Block.UpgradeValueBy(4);
    }
}