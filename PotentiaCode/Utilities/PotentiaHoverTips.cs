using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Potentia.PotentiaCode.Utilities;

public class PotentiaHoverTips
{
    public static IHoverTip Recharge()
    {
        return new HoverTip(new LocString("static_hover_tips", "POTENTIA-RECHARGE_STATIC.title"),
            new LocString("static_hover_tips", "POTENTIA-RECHARGE_STATIC.description"));
    }
}