using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Cards;

namespace Potentia.PotentiaCode.Relics;

public class LaserPistol : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<EnergyVolley>()];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player.PlayerCombatState == null) throw new InvalidOperationException("PlayerCombatState is null");
        if (player == Owner && player.PlayerCombatState.TurnNumber == 1)
        {
            Flash();
            var cards = new List<CardModel>();
            for (var index = 0; index < DynamicVars.Cards.IntValue; ++index)
                cards.Add(combatState.CreateCard<EnergyVolley>(player));
            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
        }
    }
}