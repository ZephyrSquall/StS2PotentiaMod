using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Cards;

public class ConcealedContraptions() : PotentiaCard(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust), HoverTipFactory.FromKeyword(PotentiaKeywords.Auto),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null) throw new InvalidOperationException("CombatState is null");
        foreach (Creature teammate in CombatState.GetTeammatesOf(Owner.Creature).Where(c => c.IsAlive && c.IsPlayer))
        {
            if (teammate.Player == null) throw new InvalidOperationException("teammate.Player is null");
            IEnumerable<CardModel> cards = PileType.Draw.GetPile(teammate.Player)
                .Cards.Where(c => (c.Type == CardType.Attack || c.Type == CardType.Skill) &&
                                  !c.Keywords.Contains(CardKeyword.Exhaust) &&
                                  !c.Keywords.Contains(CardKeyword.Unplayable) && (IsUpgraded
                                      ? (!c.Keywords.Contains(PotentiaKeywords.Auto) ||
                                         !c.Keywords.Contains(CardKeyword.Retain))
                                      : !c.Keywords.Contains(PotentiaKeywords.Auto)));
            CardModel? card = Owner.RunState.Rng.CombatCardSelection.NextItem(cards);

            if (card != null)
            {
                CardCmd.ApplyKeyword(card, PotentiaKeywords.Auto);
                if (IsUpgraded) CardCmd.ApplyKeyword(card, CardKeyword.Retain);
                CardCmd.Preview(card);
            }
        }
    }
}