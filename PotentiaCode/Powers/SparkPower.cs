using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Cards;
using Potentia.PotentiaCode.CombatHistoryEntries;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Powers;

public class SparkPower() : PotentiaPower, IAfterRecharged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        PotentiaHoverTips.Recharge(), HoverTipFactory.FromCard<Dissipate>()
    ];

    public override int DisplayAmount => Math.Max(0, Amount - TimesRechargedEnergyThisTurn);

    private int TimesRechargedEnergyThisTurn =>
        CombatManager.Instance.History.Entries.OfType<RechargedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner && e.Energy >= 1);

    public async Task AfterRecharged(CardModel card, int recharge, int energy)
    {
        if (card.Owner.Creature == Owner && energy >= 1 && TimesRechargedEnergyThisTurn <= Amount)
        {
            foreach (Creature teammate in CombatState.GetTeammatesOf(Owner)
                         .Where(c => c.IsAlive && c.IsPlayer && c != Owner))
            {
                if (teammate.Player == null) throw new InvalidOperationException("teammate.Player is null");
                await PlayerCmd.GainEnergy(energy, teammate.Player);
            }

            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (participants.Contains(Owner)) InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}