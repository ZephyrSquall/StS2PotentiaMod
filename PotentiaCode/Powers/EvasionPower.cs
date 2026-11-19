using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Powers;

public class EvasionPower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        Data internalData = GetInternalData<Data>();
        if (internalData.cardPlay == null && cardPlay.Card.Owner.Creature == Owner)
        {
            internalData.cardPlay = cardPlay;
            internalData.amountWhenCardStarted = Amount;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Data internalData = GetInternalData<Data>();
        if (internalData.cardPlay == cardPlay)
        {
            await CreatureCmd.GainBlock(Owner, cardPlay.Resources.EnergyValue * internalData.amountWhenCardStarted,
                ValueProp.Unpowered, null);
            internalData.cardPlay = null;
            await PowerCmd.ModifyAmount(choiceContext, this, -internalData.amountWhenCardStarted, null, null);
        }
    }

    private class Data
    {
        public CardPlay? cardPlay;
        public int amountWhenCardStarted;
    }
}