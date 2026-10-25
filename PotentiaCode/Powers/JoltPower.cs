using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Potentia.PotentiaCode.Powers;

public class JoltPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    // BeforeCardPlayed is called within OnPlayWrapper, and the X value of the card is set before OnPlayWrapper is
    // called, so ResolveEnergyXValue will get the correct value for this card play.
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner && cardPlay.Card.EnergyCost.CostsX && cardPlay.IsFirstInSeries)
            GetInternalData<Data>().xAmountsForPlayedCards.Add(cardPlay.Card, cardPlay.Card.ResolveEnergyXValue());
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (GetInternalData<Data>().xAmountsForPlayedCards.Remove(cardPlay.Card, out int xValue))
        {
            Flash();
            await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), Owner, xValue, Owner, null);
            await PowerCmd.Decrement(this);
        }
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> xAmountsForPlayedCards = [];
    }
}