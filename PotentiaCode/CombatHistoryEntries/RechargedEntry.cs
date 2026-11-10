using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Potentia.PotentiaCode.CombatHistoryEntries;

public class RechargedEntry : CombatHistoryEntry
{
    // Recharge value on the card at the instant energy was regained.
    public int Recharge { get; }

    // Amount of energy that was regained.
    public int Energy { get; }

    public override string Description =>
        $"{Actor.Player?.Character.Id.Entry} Recharged {Recharge} to regain {Energy} energy";

    public RechargedEntry(int recharge, int energy, Player player, int roundNumber, CombatSide currentSide,
        CombatHistory history, IEnumerable<Player> players) : base(player.Creature, roundNumber, currentSide, history,
        players)
    {
        Recharge = recharge;
        Energy = energy;
    }
}

public static partial class CombatHistoryExtensions
{
    public static void Recharged(this CombatHistory combatHistory, ICombatState combatState, int recharge, int energy,
        Player player) =>
        combatHistory.Add(combatState,
            new RechargedEntry(recharge, energy, player, combatState.RoundNumber, combatState.CurrentSide,
                combatHistory, combatState.Players));
}