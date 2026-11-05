using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Potions;

public sealed class ConservationBrew : PotentiaPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        if (target.Player == null) throw new InvalidOperationException("Player is null");
        Player player = target.Player;
        foreach (CardModel card in (await CardSelectCmd.FromHand(choiceContext, player,
                     new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue),
                     null, this)).ToList())
            await CardCmd.TransformToRandom(card, target.Player.RunState.Rng.CombatCardGeneration);
    }
}