using System;
using System.Collections.Generic;
using System.Linq;
using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator;

public static class SmithingMoveHelper
{
    public static List<Enum> PossibleMoves { get; } = new() { SmithingMoveTypeEnum.Punch, SmithingMoveTypeEnum.Bend, SmithingMoveTypeEnum.Upset, SmithingMoveTypeEnum.Shrink, SmithingMoveHitTypeEnum.Light, SmithingMoveHitTypeEnum.Medium, SmithingMoveHitTypeEnum.Hard, SmithingMoveTypeEnum.Draw };

    private static List<(Enum move, int pointChange)> MovesAndTheirPointsValues { get; } = [(SmithingMoveTypeEnum.Punch, 2), (SmithingMoveTypeEnum.Bend, 7), (SmithingMoveTypeEnum.Upset, 13), (SmithingMoveTypeEnum.Shrink, 16), (SmithingMoveHitTypeEnum.Light, -3), (SmithingMoveHitTypeEnum.Medium, -6), (SmithingMoveHitTypeEnum.Hard, -9), (SmithingMoveTypeEnum.Draw, -15)];

    public static int GetMovePointValue(this Enum moveType)
    {
        if (moveType is null)
            return 0;

        var foundPointChange = MovesAndTheirPointsValues.Where(entry => entry.move == moveType).Select(entry => entry.pointChange).FirstOrDefault();

        if (foundPointChange == 0)
        {
            throw new KeyNotFoundException($"Couldn't find the move {moveType} in the pre-defined moves in {nameof(SmithingMoveHelper)}.");
        }

        return foundPointChange;
    }
}
