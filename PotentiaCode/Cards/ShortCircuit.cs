using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Potentia.PotentiaCode.Powers;

namespace Potentia.PotentiaCode.Cards;

public class ShortCircuit() : PotentiaCard(1, CardType.Power, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ShockPower>(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null) throw new InvalidOperationException("CombatState is null");
        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ShockPower>(choiceContext, hittableEnemy, DynamicVars.Power<ShockPower>().BaseValue,
                Owner.Creature, this);
        }

        await PowerCmd.Apply<ShortCircuitPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<ShockPower>().UpgradeValueBy(2);
}