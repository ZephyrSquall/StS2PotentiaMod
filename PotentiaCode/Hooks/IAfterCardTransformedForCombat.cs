using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

/// <summary>
/// Runs after a player Transformed a card in a combat pile. Combat-only hook.
/// </summary>
/// <param name="choiceContext">The context that is signalled in the event of a player choice.</param>
/// <param name="card">The resulting card from the Transformation</param>
public interface IAfterCardTransformedForCombat
{
    Task AfterCardTransformedForCombat(PlayerChoiceContext choiceContext, CardModel card);
}