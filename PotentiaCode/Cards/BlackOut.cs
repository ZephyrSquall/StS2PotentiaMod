using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Potentia.PotentiaCode.Powers;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Cards;

public class BlackOut() : PotentiaCard(1, CardType.Skill, CardRarity.Rare, TargetType.RandomEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [PotentiaKeywords.Auto];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ShockPower>(4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        Creature target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (target != null)
            await PowerCmd.Apply<ShockPower>(choiceContext, target, DynamicVars.Power<ShockPower>().BaseValue,
                Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<ShockPower>().UpgradeValueBy(1);
}