using System;
using System.Collections.Generic;
using System.Linq;
using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator;

public static class SmithingMoveHelper
{
    public static List<SmithingMoveTypeEnum> AllMoves { get; } = new() { SmithingMoveTypeEnum.Punch, SmithingMoveTypeEnum.Bend, SmithingMoveTypeEnum.Upset, SmithingMoveTypeEnum.Shrink, SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Draw };
    public static List<Enum> PossibleMoves { get; } = new() { SmithingMoveTypeEnum.Punch, SmithingMoveTypeEnum.Bend, SmithingMoveTypeEnum.Upset, SmithingMoveTypeEnum.Shrink, SmithingMoveHitTypeEnum.Light, SmithingMoveHitTypeEnum.Medium, SmithingMoveHitTypeEnum.Hard, SmithingMoveTypeEnum.Draw };

    private static List<(Enum move, int pointChange)> MovesAndTheirPointsValues { get; } = [(SmithingMoveTypeEnum.Punch, 2), (SmithingMoveTypeEnum.Bend, 7), (SmithingMoveTypeEnum.Upset, 13), (SmithingMoveTypeEnum.Shrink, 16), (SmithingMoveHitTypeEnum.Light, -3), (SmithingMoveHitTypeEnum.Medium, -6), (SmithingMoveHitTypeEnum.Hard, -9), (SmithingMoveTypeEnum.Draw, -15)];

    public static int GetMovePointValue(this Enum moveType)
    {
        if (moveType is null)
            return 0;

        int foundPointChange = 0;
        foundPointChange = MovesAndTheirPointsValues.Where(entry => Enum.Equals(entry.move, moveType)).Select(entry => entry.pointChange).FirstOrDefault();

        if (foundPointChange == 0)
        {
            throw new KeyNotFoundException($"Couldn't find the move {moveType} in the pre-defined moves in {nameof(SmithingMoveHelper)}.");
        }

        return foundPointChange;
    }

    public static List<ISmithingMove> GetAppropriateMoveForMoveEnum(SmithingMoveTypeEnum move)
    {
        return move switch
        {
            SmithingMoveTypeEnum.Punch => [new PositiveSmithingMove(move)],
            SmithingMoveTypeEnum.Bend => [new PositiveSmithingMove(move)],
            SmithingMoveTypeEnum.Upset => [new PositiveSmithingMove(move)],
            SmithingMoveTypeEnum.Shrink => [new PositiveSmithingMove(move)],
            SmithingMoveTypeEnum.Hit => GetAllHitMoves(),
            SmithingMoveTypeEnum.Draw => [new NegativeSmithingMove(move)],
            _ => throw new ArgumentOutOfRangeException(nameof(move)),
        };
    }

    public static List<ISmithingMove> GetAllHitMoves()
    {
        return
        [
            MoveEnumToMove(SmithingMoveTypeEnum.Hit, SmithingMoveHitTypeEnum.Light),
            MoveEnumToMove(SmithingMoveTypeEnum.Hit, SmithingMoveHitTypeEnum.Medium),
            MoveEnumToMove(SmithingMoveTypeEnum.Hit, SmithingMoveHitTypeEnum.Hard),
        ];
    }

    public static ISmithingMove MoveEnumToMove(SmithingMoveTypeEnum move, SmithingMoveHitTypeEnum? hitMove = null)
    {
        return move switch
        {
            SmithingMoveTypeEnum.Punch => new PositiveSmithingMove(move),
            SmithingMoveTypeEnum.Bend => new PositiveSmithingMove(move),
            SmithingMoveTypeEnum.Upset => new PositiveSmithingMove(move),
            SmithingMoveTypeEnum.Shrink => new PositiveSmithingMove(move),
            SmithingMoveTypeEnum.Hit => new NegativeSmithingMoveHit(hitMove),
            SmithingMoveTypeEnum.Draw => new NegativeSmithingMove(move),
            _ => throw new ArgumentOutOfRangeException(nameof(move)),
        };
    }
}
