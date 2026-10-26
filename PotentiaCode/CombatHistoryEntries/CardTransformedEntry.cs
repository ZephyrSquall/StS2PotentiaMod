using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.CombatHistoryEntries;

public class CardTransformedEntry : CombatHistoryEntry
{
    public CardModel Card { get; }

    public override string Description =>
        $"{Actor.Player.Character.Id.Entry} transformed a card into {Card.Id.Entry} during combat";

    public CardTransformedEntry(CardModel card, int roundNumber, CombatSide currentSide, CombatHistory history,
        IEnumerable<Player> players) : base(card.Owner.Creature, roundNumber, currentSide, history, players)
    {
        Card = card;
    }
}

public static class CombatHistoryExtensions
{
    public static void CardTransformed(this CombatHistory combatHistory, ICombatState combatState, CardModel card) =>
        combatHistory.Add(combatState,
            new CardTransformedEntry(card, combatState.RoundNumber, combatState.CurrentSide, combatHistory,
                combatState.Players));
}