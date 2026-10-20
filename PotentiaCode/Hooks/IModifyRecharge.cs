using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Hooks;

public interface IModifyRecharge
{
    int ModifyRecharge(CardModel card, int energySpent, int recharge);
}