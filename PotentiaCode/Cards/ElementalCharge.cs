using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Cards;

public class ElementalCharge() : PotentiaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [PotentiaHoverTips.Recharge()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        if (play.Target.Player == null) throw new InvalidOperationException("Player is null");
        Player player = play.Target.Player;
        List<CardModel> cards = CardFactory.GetDistinctForCombat(player,
                ModelDb.AllCharacterCardPools.SelectMany(cp =>
                    cp.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                        .Where(c => c.EnergyCost.CostsX)), 3, player.RunState.Rng.CombatCardGeneration)
            .ToList();
        if (IsUpgraded)
            foreach (CardModel c in cards)
                CardCmd.Upgrade(c);

        CardModel? card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player, true);
        if (card != null)
        {
            RechargeTracker.RechargeAmountUntilPlayed[card] += 3;
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }
}