using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

/// <summary>
/// Runs after the Recharge of a card was modified. This is only run if the card is actually being played, not if just
/// for getting a preview value for Recharge, so it is safe to cause side effects here. Combat-only hook.
/// </summary>
/// <param name="card">Card whose Recharge was modified.</param>
public interface IAfterModifyingRecharge
{
    void AfterModifyingRecharge(CardModel card);
}