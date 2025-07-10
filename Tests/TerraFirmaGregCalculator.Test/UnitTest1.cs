using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator.Test;

public class UnitTest1
{
    private static List<SmithingMoveTypeEnum>[] _probableFinishingMoves =
    [
        //new() { SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Shrink, SmithingMoveTypeEnum.Draw },
        //new() { SmithingMoveTypeEnum.Bend, SmithingMoveTypeEnum.Draw, SmithingMoveTypeEnum.Draw },
        new() {  SmithingMoveTypeEnum.Shrink }
    ];

    [Fact]
    [Trait("Category", "UnitTest")]
    public void TestVariousCombinations()
    {
        List<(List<SmithingMoveTypeEnum>, int)> failedCombinations = new();

        for (int i = 5; i < 145; i++)
        {
            foreach (var finishingMoves in _probableFinishingMoves)
            {
                var backwardsDict = BiDirectionalBFS.BuildBackwardsMap(i, 64, SmithingMoveHelper.AllMoves, finishingMoves);

                var bestPath = BiDirectionalBFS.SearchForwardsAndMeet(0, i, SmithingMoveHelper.AllMoves, backwardsDict, 64);

                var sum = bestPath?.Sum(entry => entry.PointChange);

                var everythingIsRight = sum == i;

                Assert.Equal(i, sum);

                if (everythingIsRight != true)
                {

                    failedCombinations.Add((finishingMoves, i));
                }
            }
        }

        Assert.Empty(failedCombinations);
    }
}
