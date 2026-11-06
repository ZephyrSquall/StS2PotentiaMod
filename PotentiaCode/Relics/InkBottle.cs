using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Relics;

public class InkBottle : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1), new DamageVar(10, ValueProp.Unpowered), new BlockVar(10, ValueProp.Unpowered)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.EnergyCost.CostsX &&
            cardPlay.Resources.EnergySpent == DynamicVars.Energy.IntValue)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
            if (Owner.Creature.CombatState == null) throw new InvalidOperationException("CombatState is null");
            await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, DynamicVars.Damage,
                Owner.Creature);
        }
    }

    // Whenever combat starts, subscribe to Owner.PlayerCombatState.EnergyChanged to watch for whenever the owner's
    // energy changes, and start flashing if it is equal to the right amount.
    public override Task BeforeCombatStart()
    {
        if (Owner.PlayerCombatState == null) throw new InvalidOperationException("PlayerCombatState is null");
        Owner.PlayerCombatState.EnergyChanged += (_, newEnergy) =>
        {
            Status = newEnergy == DynamicVars.Energy.IntValue ? RelicStatus.Active : RelicStatus.Normal;
        };
        return Task.CompletedTask;
    }
}