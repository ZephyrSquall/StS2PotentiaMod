using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Potentia.PotentiaCode.Patches;

namespace Potentia.PotentiaCode.Cards;

public class FerromagneticFluid : PotentiaCard
{
    public FerromagneticFluid() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        RechargeTracker.RechargeAmount[this] = 2;
    }

    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        for (int i = 0; i < xValue; i++) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(1);
}