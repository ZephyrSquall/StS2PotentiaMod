using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using Potentia.PotentiaCode.Cards;

namespace Potentia.PotentiaCode.Powers;

public class SpotWeaknessPower() : CustomTemporaryPowerModelWrapper<SpotWeakness, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;

    public override LocString Description => new("powers", "TEMPORARY_STRENGTH_DOWN.description");
    protected override string SmartDescriptionLocKey => "TEMPORARY_STRENGTH_DOWN.smartDescription";
}