using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Cards;

public class Carcinogens() : PotentiaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EnergyVolley>(IsUpgraded), HoverTipFactory.FromCard<Quicksilver>(IsUpgraded),
        HoverTipFactory.FromCard<ChokingCloud>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue);
        foreach (CardModel original in await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this))
        {
            if (CombatState == null) throw new InvalidOperationException("CombatState is null");
            CardModel replacement;
            switch (original.Type)
            {
                case CardType.Attack:
                    replacement = CombatState.CreateCard<EnergyVolley>(Owner);
                    break;
                case CardType.Skill:
                    replacement = CombatState.CreateCard<Quicksilver>(Owner);
                    break;
                case CardType.Power:
                    replacement = CombatState.CreateCard<ChokingCloud>(Owner);
                    break;
                default:
                    IEnumerable<CardModel> canonicalOptions =
                    [
                        ModelDb.Card<EnergyVolley>(), ModelDb.Card<Quicksilver>(), ModelDb.Card<ChokingCloud>()
                    ];
                    CardModel canonicalReplacement =
                        Owner.RunState.Rng.CombatCardSelection.NextItem(canonicalOptions) ??
                        throw new InvalidOperationException("canonicalTargetCard is null");
                    replacement = CombatState.CreateCard(canonicalReplacement, Owner);
                    break;
            }

            if (IsUpgraded) CardCmd.Upgrade(replacement);
            await CardCmd.Transform(original, replacement);
        }
    }
}