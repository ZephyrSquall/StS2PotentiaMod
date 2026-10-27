using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class CorrosiveSlash() : PotentiaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move), new PowerVar<FrailPower>(2), new("DexterityLoss", 0),
        new CalculationBaseVar(0), new CalculationExtraVar(1),
        new CalculatedVar("CalculatedCards").WithMultiplier((_, target) =>
            target != null
                ? target.Powers.Count(p => p.TypeForCurrentAmount == PowerType.Debuff && p is not ITemporaryPower)
                : 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips = [HoverTipFactory.FromPower<FrailPower>()];
            if (DynamicVars["DexterityLoss"].BaseValue > 0) hoverTips.Add(HoverTipFactory.FromPower<DexterityPower>());
            return hoverTips;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<FrailPower>(choiceContext, play.Target, DynamicVars.Power<FrailPower>().BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, play.Target, -DynamicVars["DexterityLoss"].BaseValue,
            Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, ((CalculatedVar)DynamicVars["CalculatedCards"]).Calculate(play.Target),
            Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["DexterityLoss"].UpgradeValueBy(2);
    }
}