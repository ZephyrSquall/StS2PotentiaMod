using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Powers;

public class ConservePower() : PotentiaPower, IModifyRecharge, IAfterModifyingRecharge
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    // This is a constant to ensure the value is synced with the Conserve card.
    public const int Recharge = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Recharge", Recharge)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [PotentiaHoverTips.Recharge()];

    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == Applier &&
            (cardPlay.Card.TargetType == TargetType.AllEnemies ||
             (cardPlay.Card.TargetType == TargetType.AnyEnemy && cardPlay.Target == Owner)))
            GetInternalData<Data>().playedAttacks.Add(cardPlay.Card);
        return Task.CompletedTask;
    }

    public int ModifyRecharge(CardModel card, int energySpent, int recharge, Creature? target, bool isPreview)
    {
        if (isPreview)
        {
            return card.Type == CardType.Attack && card.Owner.Creature == Applier &&
                   (card.TargetType == TargetType.AllEnemies ||
                    (card.TargetType == TargetType.AnyEnemy && target == Owner))
                ? recharge + DynamicVars["Recharge"].IntValue
                : recharge;
        }

        return GetInternalData<Data>().playedAttacks.Remove(card)
            ? recharge + DynamicVars["Recharge"].IntValue
            : recharge;
    }

    public async Task AfterModifyingRecharge(CardModel card)
    {
        await PowerCmd.Decrement(this);
    }

    private class Data
    {
        public readonly HashSet<CardModel> playedAttacks = [];
    }
}