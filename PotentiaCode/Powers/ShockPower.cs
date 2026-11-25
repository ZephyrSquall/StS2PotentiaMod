using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Powers;

public class ShockPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    protected override object InitInternalData() => new Data();

    public override Task AfterEnergySpent(CardModel card, int amount)
    {
        if (amount > 0 && (card.Owner.Creature == Applier ||
                           Owner.Powers.Any(p => p is ShockingGraspPower && p.Applier == Applier)))
        {
            // Damage can't be dealt during this hook as damage requires a PlayerChoiceContext. So, make a record of the
            // triggering card and the damage it should have dealt. This can then be checked later in the
            // AfterCardPlayed hook to deal damage then.
            int damage = amount * Amount;
            GetInternalData<Data>().damageAmountsForPlayedCards.Add(card, damage);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // If the Applier is dead, the Damage command wouldn't deal any damage anyway because the source of the damage
        // cannot deal damage anymore. But the animations would still play, so this must still be skipped in that case.
        if (GetInternalData<Data>().damageAmountsForPlayedCards.Remove(cardPlay.Card, out int damage) &&
            Applier != null && Applier.IsAlive)
        {
            // Energy Potion effect
            NCombatRoom.Instance?.PlaySplashVfx(Owner, new Color("f2e35c"));
            await Cmd.Wait(0.2f);
            IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, Owner, damage,
                ValueProp.Unpowered, Applier, null, null);

            if (Applier.Player != null)
                foreach (DamageResult damageResult in damageResults)
                    if (damageResult.Receiver.IsEnemy)
                    {
                        if (Applier.Player.PlayerCombatState == null)
                            throw new InvalidOperationException("Applier.Player.PlayerCombatState is null");
                        PotentiaSpireFields.ShockDamageDealtToEnemies[Applier.Player.PlayerCombatState] +=
                            damageResult.TotalDamage + damageResult.OverkillDamage;
                    }
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner) && !Owner.IsDead &&
            (Applier == null || !Applier.HasPower<ShortCircuitPower>()))
            await PowerCmd.Decrement(this);
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> damageAmountsForPlayedCards = new();
    }
}