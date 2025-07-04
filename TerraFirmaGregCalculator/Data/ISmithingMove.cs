using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TerraFirmaGregCalculator.Data;

public interface ISmithingMove
{
    public SmithingMoveTypeEnum MoveType { get; set; }
    public int PointChange { get; set; }

    public string GetSmithingMoveName();
}

public class ISmithingMoveComparer : IEqualityComparer<ISmithingMove?>
{
    public bool Equals(ISmithingMove? move1, ISmithingMove? move2)
    {
        return move1?.PointChange == move2?.PointChange
            && move1?.MoveType == move2?.MoveType;
    }

    public int GetHashCode([DisallowNull] ISmithingMove obj)
    {
        return 10;
    }
}