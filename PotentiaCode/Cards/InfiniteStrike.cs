using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Potentia.PotentiaCode.Cards;

public class InfiniteStrike() : PotentiaCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private int _increasedReplay = 0;

    [SavedProperty]
    public int IncreasedReplay
    {
        get => _increasedReplay;
        set
        {
            // Increase the card's Replay by the difference between the old increasedReplay amount and the new
            // increasedReplay amount. This makes sure that if the card gets Replay from other sources, that additional
            // Replay isn't overwritten. When loading a save, the old amount is 0, so this just sets this card's Replay
            // to the new amount, which is the saved amount.
            BaseReplayCount += value - _increasedReplay;
            _increasedReplay = value;
        }
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move), new IntVar("Replay", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> hoverTips = [HoverTipFactory.Static(StaticHoverTip.Fatal)];
            if (BaseReplayCount == 0) hoverTips.Add(HoverTipFactory.Static(StaticHoverTip.ReplayStatic));
            return hoverTips;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        List<Creature> fatalKills = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .ExecuteAndCheckFatal(choiceContext);

        for (int i = 0; i < fatalKills.Count; i++)
        {
            int extraReplay = DynamicVars["Replay"].IntValue;
            BuffFromPlay(extraReplay);
            if (DeckVersion is InfiniteStrike deckVersion) deckVersion.BuffFromPlay(extraReplay);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);

    private void BuffFromPlay(int extraReplay)
    {
        IncreasedReplay += extraReplay;
    }
}