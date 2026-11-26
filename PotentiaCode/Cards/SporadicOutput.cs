using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

public class SporadicOutput : PotentiaCard
{
    public SporadicOutput() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        RechargeTracker.RechargeAmount[this] = 1;
    }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();

        // Skip bringing up the selection screen if X is 0 (otherwise a screen will come up asking the player to pick 0
        // cards).
        if (xValue > 0)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, xValue);
            foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this))
            {
                CardPileAddResult result =
                    await CardCmd.TransformToRandom(card, Owner.RunState.Rng.CombatCardGeneration);
                if (IsUpgraded && result.success) CardCmd.Upgrade(result.cardAdded);
            }
        }
    }
}