using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Character;

namespace Potentia.PotentiaCode.Relics;

public class AutumnsTouch : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner)
        {
            Flash();
            foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, player,
                         new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue),
                         null, this))
            {
                switch (card.Rarity)
                {
                    case CardRarity.Common:
                        IEnumerable<CardModel> commonTransformOptions = CardFactory
                            .GetDefaultTransformationOptions(card, true)
                            .Where(c => c.Rarity == CardRarity.Uncommon);
                        CardTransformation commonCardTransformation =
                            new CardTransformation(card, commonTransformOptions);
                        await CardCmd.Transform(commonCardTransformation.Yield(),
                            player.RunState.Rng.CombatCardSelection);
                        break;
                    case CardRarity.Uncommon:
                        IEnumerable<CardModel> uncommonTransformOptions = CardFactory
                            .GetDefaultTransformationOptions(card, true)
                            .Where(c => c.Rarity == CardRarity.Rare);
                        CardTransformation uncommonCardTransformation =
                            new CardTransformation(card, uncommonTransformOptions);
                        await CardCmd.Transform(uncommonCardTransformation.Yield(),
                            player.RunState.Rng.CombatCardSelection);
                        break;
                    case CardRarity.Rare:
                        IEnumerable<CardModel> rareTransformOptions = CardFactory
                            .GetDefaultTransformationOptions(card, true)
                            .Where(c => c.Rarity == CardRarity.Rare);
                        CardTransformation rareCardTransformation = new CardTransformation(card, rareTransformOptions);
                        IEnumerable<CardPileAddResult> rareTransformResult =
                            await CardCmd.Transform(rareCardTransformation.Yield(),
                                player.RunState.Rng.CombatCardSelection);
                        foreach (CardPileAddResult result in rareTransformResult)
                        {
                            if (result.success) CardCmd.Upgrade(result.cardAdded);
                        }

                        break;
                    default:
                        // Note that only checking CardModel.CanBeGeneratedInCombat in this branch is not a mistake, as
                        // the CardFactory.GetDefaultTransformationOptions method that the other branches use already
                        // checks CanBeGeneratedInCombat internally if the isInCombat argument is true.
                        IEnumerable<CardModel> otherTransformOptions = ModelDb.CardPool<PotentiaCardPool>()
                            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                            .Where(c => c.Rarity == CardRarity.Common && c.CanBeGeneratedInCombat);
                        CardTransformation otherCardTransformation =
                            new CardTransformation(card, otherTransformOptions);
                        await CardCmd.Transform(otherCardTransformation.Yield(),
                            player.RunState.Rng.CombatCardSelection);
                        break;
                }
            }
        }
    }
}