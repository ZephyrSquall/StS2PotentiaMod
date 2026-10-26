using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

/// <summary>
/// Modify the amount of Recharge that a card will have. Note that this is called both to get a preview value to put on
/// the card, and to actually determine how much Recharge the card has when played, so avoid side effects in this hook.
/// If you want something to happen when actually modifying Recharge, look at AfterModifyingRecharge. Combat-only hook.
/// </summary>
/// <param name="card">Card that is getting Recharge.</param>
/// <param name="energySpent">Energy spent to play this card (or predicted energy spend for preview values).</param>
/// <param name="recharge">Original amount of Recharge the card would have.</param>
/// <returns>New amount of Recharge the card will have.</returns>
public interface IModifyRecharge
{
    int ModifyRecharge(CardModel card, int energySpent, int recharge);
}