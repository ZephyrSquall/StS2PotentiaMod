using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.CombatHistoryEntries;
using Potentia.PotentiaCode.Hooks;

namespace Potentia.PotentiaCode.Powers;

public class ElectromagnetismPower() : PotentiaPower, IAfterCardTransformedForCombat
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Transform), HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    public override int DisplayAmount => Math.Max(0, Amount - TimesTransformedThisTurn);

    private int TimesTransformedThisTurn =>
        CombatManager.Instance.History.Entries.OfType<CardTransformedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner);

    public async Task AfterCardTransformedForCombat(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature == Owner && Owner.Player != null && TimesTransformedThisTurn <= Amount)
        {
            Flash();
            foreach (CardModel chosenCard in await CardSelectCmd.FromHand(choiceContext, Owner.Player,
                         new CardSelectorPrefs(SelectionScreenPrompt, 0, 1),
                         c => !c.Keywords.Contains(CardKeyword.Retain), this))
                CardCmd.ApplyKeyword(chosenCard, CardKeyword.Retain);
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (participants.Contains(Owner)) InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}