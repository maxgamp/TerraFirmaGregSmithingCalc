namespace TerraFirmaGregCalculator.Data;

public class CalculatorSettings
{
    public int? MaxDepth { get; set; }
    public TreeTraversalModeEnum TreeTraversalMode { get; set; } = TreeTraversalModeEnum.BiDirectional;
}
