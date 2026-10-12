using BaseLib.Abstracts;
using Potentia.PotentiaCode.Extensions;
using Godot;

namespace Potentia.PotentiaCode.Character;

public class PotentiaRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Potentia.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}