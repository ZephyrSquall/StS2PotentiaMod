using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Potentia.PotentiaCode.Hooks;

namespace Potentia.PotentiaCode.Powers;

public class KineticPotentialPower() : PotentiaPower, IAfterTransform
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Transform), HoverTipFactory.FromPower<VigorPower>()
    ];

    public async Task AfterTransform(ICombatState combatState, CardModel card, Player? transformer)
    {
        if (transformer != null && transformer.Creature == Owner)
        {
            Flash();
            await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
        }
    }
}