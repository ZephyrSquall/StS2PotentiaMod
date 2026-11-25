using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Potentia.PotentiaCode.Utilities;

public class PotentiaSpireFields
{
    // This is used only for Flash to track total Shock damage dealt to enemies this combat. If other cards start caring
    // about this, especially in subtly different ways like caring about the Shock damage this turn, may be better to
    // refactor this into a CombatHistoryEvent.
    public static readonly SpireField<PlayerCombatState, int> ShockDamageDealtToEnemies = new(() => 0);
}