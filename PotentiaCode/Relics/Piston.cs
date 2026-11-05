using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Relics;

public class Piston : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState == null) throw new InvalidOperationException("PlayerCombatState is null");
        if (player == Owner && player.PlayerCombatState.TurnNumber == 1)
        {
            Flash();
            foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, Owner,
                         new CardSelectorPrefs(SelectionScreenPrompt, 1), c => !c.Keywords.Contains(CardKeyword.Retain),
                         this))
                CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }
    }
}