namespace TerraFirmaGregCalculator.Data;

public interface ISmithingMove
{
    public SmithingMoveTypeEnum MoveType { get; set; }
    public int PointChange { get; set; }

    public string GetSmithingMoveName();
}
