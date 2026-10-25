using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class SugarAndSpice() : PotentiaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<UltimateStrike>(IsUpgraded), HoverTipFactory.FromCard<UltimateDefend>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        List<CardModel> strikesAndDefends = PileType.Hand.GetPile(Owner)
            .Cards.Where(card =>
                card.Rarity == CardRarity.Basic &&
                (card.Tags.Contains(CardTag.Strike) || card.Tags.Contains(CardTag.Defend)))
            .ToList();

        foreach (CardModel strikeOrDefend in strikesAndDefends)
        {
            CardModel? newCard = null;
            if (strikeOrDefend.Tags.Contains(CardTag.Strike))
                newCard = CombatState.CreateCard<UltimateStrike>(Owner);
            else if (strikeOrDefend.Tags.Contains(CardTag.Defend))
                newCard = CombatState.CreateCard<UltimateDefend>(Owner);

            if (newCard != null)
            {
                if (IsUpgraded) CardCmd.Upgrade(newCard);
                await CardCmd.Transform(strikeOrDefend, newCard);
            }
        }
    }
}