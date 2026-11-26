using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.CombatHistoryEntries;
using Potentia.PotentiaCode.Hooks;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Cards;

public class SolarPanel() : PotentiaCard(4, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IAfterRecharged
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12, ValueProp.Move), new EnergyVar(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [PotentiaHoverTips.Recharge()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4);

    private int TimesRechargedEnergyThisTurn =>
        CombatManager.Instance.History.Entries.OfType<RechargedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner.Creature && e.Energy >= 1);

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this && !IsClone)
            EnergyCost.AddThisTurn(-TimesRechargedEnergyThisTurn * DynamicVars.Energy.IntValue);
        return Task.CompletedTask;
    }

    public Task AfterRecharged(CardModel card, int recharge, int energy)
    {
        if (card.Owner == Owner && energy >= 1) EnergyCost.AddThisTurn(-DynamicVars.Energy.IntValue);
        return Task.CompletedTask;
    }
}