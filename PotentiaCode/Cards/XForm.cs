using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Potentia.PotentiaCode.Cards;

public class XForm() : PotentiaCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override bool IsPlayable => EnergyCost.GetAmountToSpend() >= DynamicVars.Energy.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        if (IsUpgraded) xValue++;
        // Base values are taken directly from the cards that apply them, so this list should stay up-to-date if the
        // base game changes these values. The way the game defines the base values for each form card is inconsistent,
        // so it looks like I'm being inconsistent below, but I'm merely copying exactly how the base game does it.
        IReadOnlyList<(PowerModel power, decimal baseValue)> formPowerOptionsWithBaseValues =
        [
            (ModelDb.Power<DemonFormPower>(), ModelDb.Card<DemonForm>().DynamicVars["StrengthPower"].BaseValue),
            (ModelDb.Power<SerpentFormPower>(),
                ModelDb.Card<SerpentForm>().DynamicVars["SerpentFormPower"].BaseValue),
            (ModelDb.Power<VoidFormPower>(), ModelDb.Card<VoidForm>().DynamicVars["VoidFormPower"].BaseValue),
            (ModelDb.Power<ReaperFormPower>(), 1),
            (ModelDb.Power<EchoFormPower>(), ModelDb.Card<EchoForm>().DynamicVars[nameof(EchoForm)].BaseValue)
        ];

        for (int i = 0; i < xValue; i++)
        {
            (PowerModel canonicalFormPower, decimal baseValue) =
                Owner.RunState.Rng.CombatCardSelection.NextItem(formPowerOptionsWithBaseValues);
            PowerModel formPower = canonicalFormPower.ToMutable();
            await PowerCmd.Apply(choiceContext, formPower, Owner.Creature, baseValue, Owner.Creature, this);
        }
    }
}