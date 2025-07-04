using System;
using System.Collections.Generic;
using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator;

public static class SmithingMoveHelper
{
    public static List<Enum> PossibleMoves { get; } = new() { SmithingMoveTypeEnum.Punch, SmithingMoveTypeEnum.Bend, SmithingMoveTypeEnum.Upset, SmithingMoveTypeEnum.Shrink, SmithingMoveHitTypeEnum.Light, SmithingMoveHitTypeEnum.Medium, SmithingMoveHitTypeEnum.Hard, SmithingMoveTypeEnum.Draw };

}
