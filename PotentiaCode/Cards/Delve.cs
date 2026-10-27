using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Potentia.PotentiaCode.Cards;

public class Delve() : PotentiaCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int maxCount = Math.Min(DynamicVars.Cards.IntValue,
            CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count);
        if (maxCount > 0)
            await CardPileCmd.Add(
                await CardSelectCmd.FromCombatPile(choiceContext, PileType.Discard.GetPile(Owner), Owner,
                    new CardSelectorPrefs(SelectionScreenPrompt, maxCount)), PileType.Hand);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}