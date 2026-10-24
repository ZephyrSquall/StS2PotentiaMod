using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Relics;

public class UnnamedStarterRelicUpgrade() : PotentiaRelic, IModifyRecharge
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Recharge", 3)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [PotentiaHoverTips.Recharge()];

    private bool HasSpentEnergyOnXCardThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished.Any(e =>
            e.HappenedThisTurn(Owner.Creature.CombatState) && e.CardPlay.Player == Owner &&
            e.CardPlay.Resources.EnergySpent > 0 && e.CardPlay.Card.EnergyCost.CostsX);

    public int ModifyRecharge(CardModel card, int energySpent, int recharge)
    {
        if (card.Owner == Owner && card.EnergyCost.CostsX && energySpent > 0 && !HasSpentEnergyOnXCardThisTurn)
            return recharge + DynamicVars["Recharge"].IntValue;
        return recharge;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (HasSpentEnergyOnXCardThisTurn) Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature)) Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}