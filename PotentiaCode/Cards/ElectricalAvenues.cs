using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

public class ElectricalAvenues : PotentiaCard
{
    public ElectricalAvenues() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        RechargeTracker.RechargeAmount[this] = 1;
        RechargeTracker.IsRechargeIncreasedOnUpgrade[this] = true;
    }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        int cards = xValue * DynamicVars.Cards.IntValue;
        foreach (CardModel card in await CardPileCmd.Draw(choiceContext, cards, Owner))
            CardCmd.ApplySingleTurnRetain(card);
    }

    protected override void OnUpgrade() => RechargeTracker.RechargeAmount[this] += 1;
    protected override void AfterDowngraded() => RechargeTracker.RechargeAmount[this] -= 1;
}