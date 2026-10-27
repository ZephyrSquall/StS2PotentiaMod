using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.CombatHistoryEntries;

namespace Potentia.PotentiaCode.Cards;

public class HeatRadiance() : PotentiaCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move),];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

    private bool HasTransformedThisTurn =>
        CombatManager.Instance.History.Entries.OfType<CardTransformedEntry>()
            .Any(e => e.HappenedThisTurn(Owner.Creature.CombatState) && e.Card.Owner == Owner);

    protected override bool ShouldGlowGoldInternal => HasTransformedThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card == this && HasTransformedThisTurn)
        {
            modifiedCost = 0;
            return true;
        }

        modifiedCost = originalCost;
        return false;
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}