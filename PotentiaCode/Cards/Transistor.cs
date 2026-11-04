using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Patches;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Cards;

public class Transistor() : PotentiaCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(25, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PotentiaKeywords.Auto), HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        if (IsUpgraded)
        {
            CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
                card => !card.Keywords.Contains(PotentiaKeywords.Auto), this)).FirstOrDefault();
            if (card != null) CardCmd.ApplyKeyword(card, PotentiaKeywords.Auto);
        }
        else
        {
            CardModel? card =
                (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, card => !card.IsAutoThisTurn(), this))
                .FirstOrDefault();
            if (card != null) AutoTracker.HasSingleTurnAuto[card] = true;
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5);
}