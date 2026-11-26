using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Cards;

public class Discord() : PotentiaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        List<CardModel> cards = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner), Owner,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue))).ToList();
        foreach (CardModel card in cards)
            await CardCmd.TransformToRandom(card, Owner.RunState.Rng.CombatCardGeneration);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}