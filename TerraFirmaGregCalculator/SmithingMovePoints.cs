using System;

namespace TerraFirmaGregCalculator;

public class SmithingMovePoints : Attribute
{
    public int PointValue { get; set; }

    public SmithingMovePoints(int pointValue)
    {
        PointValue = pointValue;
    }
}
