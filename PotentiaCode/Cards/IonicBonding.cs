using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class IonicBonding() : PotentiaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0), new CalculationExtraVar(2),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) =>
        {
            // if (card.Pile == null) return 0;
            int energySpentThisTurn = CombatManager.Instance.History.Entries.OfType<EnergySpentEntry>()
                .Where(e => e.HappenedThisTurn(card.CombatState) && e.Actor.Player == card.Owner)
                .Sum(c => c.Amount);
            CardPlay? currentCardPlay = CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
                .LastOrDefault(e =>
                    e.HappenedThisTurn(card.CombatState) && e.Actor.Player == card.Owner && e.CardPlay.Card == card)
                ?.CardPlay;
            if (currentCardPlay != null && !CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>()
                    .Any(e => e.HappenedThisTurn(card.CombatState) && e.Actor.Player == card.Owner &&
                              e.CardPlay == currentCardPlay))
                energySpentThisTurn -= currentCardPlay.Resources.EnergySpent;
            return energySpentThisTurn;
        })
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(play.Target),
            DynamicVars.CalculatedBlock.Props, play);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}