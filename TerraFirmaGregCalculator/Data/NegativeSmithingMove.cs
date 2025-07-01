using System;

namespace TerraFirmaGregCalculator.Data;

public class NegativeSmithingMove : ISmithingMove
{
    public SmithingMoveTypeEnum MoveType { get; set; }
    public int PointChange { get; set; }

    public NegativeSmithingMove(SmithingMoveTypeEnum moveType, int pointChange)
    {
        MoveType = moveType;
        PointChange = -Math.Abs(pointChange);
    }

    public NegativeSmithingMove(SmithingMoveTypeEnum moveType) : this(moveType, moveType.GetMovePointValue()) { }

    public string GetSmithingMoveName()
    {
        return Enum.GetName(MoveType) ?? "null";
    }
}
