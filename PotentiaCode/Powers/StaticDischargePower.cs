using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Potentia.PotentiaCode.Powers;

public class StaticDischargePower() : PotentiaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount =>
        DynamicVars.Energy.IntValue - GetInternalData<Data>().energySpent % DynamicVars.Energy.IntValue;

    // This is a constant to ensure the value is synced with the Static Discharge card.
    public const int EnergyThreshold = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(EnergyThreshold)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override object InitInternalData() => new Data();

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature == Owner && amount > 0)
        {
            Data data = GetInternalData<Data>();
            data.energySpent += amount;
            int triggers = data.energySpent / DynamicVars.Energy.IntValue - data.triggerCount;
            if (triggers > 0)
            {
                Flash();
                int shock = Amount * triggers;
                data.shockAmountsForPlayedCards.Add(card, shock);
                data.triggerCount += triggers;
            }

            InvokeDisplayAmountChanged();
        }
    }

    // Applying Shock must be delayed until after the AfterEnergySpent hook. This is because Shock itself checks its
    // own amount during the AfterEnergySpent hook, so this leads to inconsistent results depending on whether Static
    // Discharge's or Shock's AfterEnergySpent hook is run first. Delaying until AfterCardPlayed gives consistency with
    // Potentia's other "whenever you spend energy" effects like Conservation of Energy and Shock itself.
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (GetInternalData<Data>().shockAmountsForPlayedCards.Remove(cardPlay.Card, out int shock))
            await PowerCmd.Apply<ShockPower>(choiceContext, CombatState.HittableEnemies, shock, Owner, null);
    }

    private class Data
    {
        public int energySpent;
        public int triggerCount;
        public readonly Dictionary<CardModel, int> shockAmountsForPlayedCards = new();
    }
}