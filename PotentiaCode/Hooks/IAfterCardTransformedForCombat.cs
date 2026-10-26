using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

/// <summary>
/// Runs after a player Transformed a card in a combat pile. Combat-only hook.
/// </summary>
/// <param name="card">The resulting card from the Transformation</param>
public interface IAfterCardTransformedForCombat
{
    Task AfterCardTransformedForCombat(CardModel card);
}