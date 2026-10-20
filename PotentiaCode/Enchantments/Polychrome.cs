using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using Potentia.PotentiaCode.Utilities;

namespace Potentia.PotentiaCode.Enchantments;

public class Polychrome : CustomEnchantmentModel
{
    protected override string? CustomIconPath => "res://Potentia/images/enchantments/polychrome.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(PotentiaKeywords.Prismatic), HoverTipFactory.Static(StaticHoverTip.Transform)
    ];

    protected override void OnEnchant()
    {
        Card.AddKeyword(PotentiaKeywords.Prismatic);
    }
}