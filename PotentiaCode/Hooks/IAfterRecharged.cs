using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

/// <summary>
/// Runs after a card's Recharge effect was triggered. This is only run if the card is actually being played, not if
/// just for getting a preview value for Recharge, so it is safe to cause side effects here. This will only run if
/// Recharge is greater than 0, but may run if 0 energy was actually regained. Combat-only hook.
/// </summary>
/// <param name="card">Card whose Recharge was triggered.</param>
/// <param name="recharge">Amount of Recharge on the card when it triggered.</param>
/// <param name="energy">Amount of energy that was regained.</param>
public interface IAfterRecharged
{
    Task AfterRecharged(CardModel card, int recharge, int energy);
}