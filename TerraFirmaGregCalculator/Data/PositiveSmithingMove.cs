using System;

namespace TerraFirmaGregCalculator.Data;

public class PositiveSmithingMove : ISmithingMove
{
    public SmithingMoveTypeEnum MoveType { get; set; }
    public int PointChange { get; set; }

    public PositiveSmithingMove(SmithingMoveTypeEnum moveType, int pointChange)
    {
        MoveType = moveType;
        PointChange = pointChange;
    }

    public PositiveSmithingMove(SmithingMoveTypeEnum moveType) : this(moveType, moveType.GetMovePointValue()) { }

    public string GetSmithingMoveName()
    {
        return Enum.GetName(MoveType) ?? "null";
    }
}
