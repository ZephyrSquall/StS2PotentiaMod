using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Potentia.PotentiaCode.Character;
using Potentia.PotentiaCode.Extensions;

namespace Potentia.PotentiaCode.Potions;

[Pool(typeof(PotentiaPotionPool))]
public abstract class PotentiaPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}