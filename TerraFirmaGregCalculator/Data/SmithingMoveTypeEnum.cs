namespace TerraFirmaGregCalculator.Data;

public enum SmithingMoveTypeEnum
{
    [SmithingMovePoints(2)]
    Punch,
    [SmithingMovePoints(7)]
    Bend,
    [SmithingMovePoints(13)]
    Upset,
    [SmithingMovePoints(16)]
    Shrink,
    Hit,
    [SmithingMovePoints(-15)]
    Draw
}
