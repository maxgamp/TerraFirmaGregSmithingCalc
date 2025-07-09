
using System.Collections.Concurrent;
using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator.Test;

public class UnitTest1
{
    private static List<SmithingMoveTypeEnum>[] _probableFinishingMoves = [new() { SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Shrink, SmithingMoveTypeEnum.Draw }, new() { }];

    [Fact]
    [Trait("Category", "UnitTest")]
    public void TestVariousCombinations()
    {
        ConcurrentDictionary<List<SmithingMoveTypeEnum>, int> failedCombinations = new ConcurrentDictionary<List<SmithingMoveTypeEnum>, int>();

        var x = Parallel.For(1, 139, (i) =>
        {
            foreach (var finishingMoves in _probableFinishingMoves)
            {
                var result = Program.CalculateSmithingOrder(i, finishingMoves, (_, _) => { });

                var everythingIsRight = result.All(entry => entry.MoveList.Last().CurrentPoints == 0);

                if (everythingIsRight)
                {
                    failedCombinations.TryAdd(finishingMoves, i);
                }
            }

        });

        Assert.Empty(failedCombinations);
    }
}
