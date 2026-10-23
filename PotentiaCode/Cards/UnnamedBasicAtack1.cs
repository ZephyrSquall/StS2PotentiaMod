using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class UnnamedBasicSkill1()
    : PotentiaCard(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy), ITranscendenceCard
{
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<UnnamedAncientSkill1>();

    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override bool ShouldGlowGoldInternal => EnergyCost.GetAmountToSpend() == DynamicVars.Energy.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move), new EnergyVar(1),
        new BlockVar(8, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithValueProp(DynamicVars.Damage.Props)
            .WithHitCount(xValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithAttackerAnim("Cast", Owner.Character.CastAnimDelay)
            .Execute(choiceContext);

        if (play.Resources.EnergySpent == DynamicVars.Energy.IntValue)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(3);
    }
}