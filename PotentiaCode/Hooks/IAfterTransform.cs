using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

public interface IAfterTransform
{
    Task AfterTransform(ICombatState combatState, CardModel card, Player? transformer);
}