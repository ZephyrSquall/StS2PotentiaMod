using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class Refrain() : PotentiaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();

        for (int i = 0; i < xValue; i++) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        LocString prompt = SelectionScreenPrompt;
        prompt.Add("Amount", xValue);
        CardSelectorPrefs prefs = new CardSelectorPrefs(prompt, xValue);
        foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
                     c => !c.Keywords.Contains(CardKeyword.Retain), this))
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}