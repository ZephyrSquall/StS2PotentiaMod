using BaseLib.Abstracts;
using BaseLib.Utils;
using Potentia.PotentiaCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Potentia.PotentiaCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
[Pool(typeof(PotentiaCardPool))]
public abstract class PotentiaCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // The overrides for CustomPortraitPath, PortraitPath, BetaPortraitPath are removed so Art Roller can handle all the
    // art for this mod.
}