using System;

namespace TerraFirmaGregCalculator.Data;

public class NegativeSmithingMoveHit : ISmithingMove
{
    public SmithingMoveTypeEnum MoveType { get; set; }
    public SmithingMoveHitTypeEnum? HitType { get; set; }
    public int PointChange { get; set; }

    public NegativeSmithingMoveHit(SmithingMoveHitTypeEnum? hitType, int? pointChange)
    {
        MoveType = SmithingMoveTypeEnum.Hit;
        HitType = hitType;
        PointChange = pointChange ?? 0;
    }

    public NegativeSmithingMoveHit(SmithingMoveHitTypeEnum? hitType) : this(hitType, hitType?.GetMovePointValue()) { }

    public string GetSmithingMoveName()
    {
        var hitName = "null";
        if (HitType != null)
            hitName = Enum.GetName((SmithingMoveHitTypeEnum)HitType);

        return hitName + " " + Enum.GetName(MoveType);
    }
}
