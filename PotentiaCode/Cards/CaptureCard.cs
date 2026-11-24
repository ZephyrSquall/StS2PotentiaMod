using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Cards;

public class CaptureCard() : PotentiaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        IEnumerable<CardModel> cards = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => !c.Keywords.Contains(CardKeyword.Retain), this);
        foreach (CardModel card in cards) CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}