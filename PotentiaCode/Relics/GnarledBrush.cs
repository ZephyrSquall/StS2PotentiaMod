using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Potentia.PotentiaCode.Enchantments;

namespace Potentia.PotentiaCode.Relics;

public class GnarledBrush() : PotentiaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<Polychrome>();

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 0, DynamicVars.Cards.IntValue)
            {
                Cancelable = false, RequireManualConfirmation = true
            };
        Polychrome canonicalPolychrome = ModelDb.Enchantment<Polychrome>();
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalPolychrome, 1, prefs))
        {
            CardCmd.Enchant(canonicalPolychrome.ToMutable(), card, 1M);
            NCardEnchantVfx child = NCardEnchantVfx.Create(card);
            if (child != null) NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(child);
        }
    }
}