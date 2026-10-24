using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Powers;

public class ConservationOfEnergyPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    public override Task AfterEnergySpent(CardModel card, int amount)
    {
        if (amount > 0 && card.EnergyCost.CostsX && card.Owner.Creature == Owner)
        {
            // For consistency with the Shock power and cards like After Image, delay the Block gain until the
            // AfterCardPlayed hook.
            int block = amount * Amount;
            GetInternalData<Data>().blockAmountsForPlayedCards.Add(card, block);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (GetInternalData<Data>().blockAmountsForPlayedCards.Remove(cardPlay.Card, out int block))
            await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null, true);
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> blockAmountsForPlayedCards = new();
    }
}