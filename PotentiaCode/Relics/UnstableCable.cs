using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Relics;

public class UnstableCable : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ShockPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        // If the power amount exactly equals the added amount, or the power amount minus the added amount would make
        // the power a buff, then this application just newly added the debuff.
        if (applier == Owner.Creature && power.Owner.IsEnemy && power is not ITemporaryPower &&
            power.GetTypeForAmount(amount) == PowerType.Debuff &&
            (power.Amount == amount || power.GetTypeForAmount(power.Amount - amount) == PowerType.Buff))
            await PowerCmd.Apply<ShockPower>(choiceContext, power.Owner, DynamicVars.Power<ShockPower>().BaseValue,
                Owner.Creature, null);
    }
}