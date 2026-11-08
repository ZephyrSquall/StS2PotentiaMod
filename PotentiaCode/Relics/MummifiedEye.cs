using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace Potentia.PotentiaCode.Relics;

public class MummifiedEye : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatManager.Instance.IsInProgress && cardPlay.Card.Owner == Owner && cardPlay.Card.EnergyCost.CostsX &&
            cardPlay.Resources.EnergySpent > 0)
        {
            // This logic is directly copied from Mummified Hand to ensure this relic selects cards in the same priority
            // order.
            Rng combatCardSelection = Owner.RunState.Rng.CombatCardSelection;
            IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(Owner).Cards;
            List<CardModel> list = cards
                .Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.None) > 0 || c.BaseStarCost > 0)
                .ToList();
            (((combatCardSelection.NextItem(list.Where(c => c.CostsEnergyOrStars(true))) ??
               combatCardSelection.NextItem(cards.Where(c => c.CostsEnergyOrStars(true)))) ??
              combatCardSelection.NextItem(list)) ?? combatCardSelection.NextItem(cards))?.SetToFreeThisTurn();
        }

        return Task.CompletedTask;
    }
}