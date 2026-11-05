using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Potions;

public sealed class SerumX : PotentiaPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3), new IntVar("Recharge", 1)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [PotentiaHoverTips.Recharge()];

    protected override Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        if (target.Player?.PlayerCombatState == null)
            throw new InvalidOperationException("Player or PlayerCombatState is null");
        foreach (CardModel card in target.Player.PlayerCombatState.AllCards.Where(c =>
                     c.EnergyCost.CostsX || c.EnergyCost.GetAmountToSpend() >= DynamicVars.Energy.IntValue))
            RechargeTracker.RechargeAmount[card]++;
        return Task.CompletedTask;
    }
}