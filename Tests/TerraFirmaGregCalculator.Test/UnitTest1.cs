using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator.Test;

public class UnitTest1
{
    private static List<SmithingMoveTypeEnum>[] _probableFinishingMoves =
    [
        new() { SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Shrink, SmithingMoveTypeEnum.Draw },
        new() { SmithingMoveTypeEnum.Bend, SmithingMoveTypeEnum.Draw, SmithingMoveTypeEnum.Draw },
        new() {  SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Hit },
        new() {  SmithingMoveTypeEnum.Hit, SmithingMoveTypeEnum.Draw, SmithingMoveTypeEnum.Hit },
        new() {  SmithingMoveTypeEnum.Upset, SmithingMoveTypeEnum.Draw, SmithingMoveTypeEnum.Punch },
        new() {  SmithingMoveTypeEnum.Shrink },
        new() {  SmithingMoveTypeEnum.Upset },
        new() {  SmithingMoveTypeEnum.Hit }
    ];

    [Fact]
    [Trait("Category", "UnitTest")]
    public void TestVariousCombinations()
    {
        for (int i = 5; i < 149; i++)
        {
            foreach (var finishingMoves in _probableFinishingMoves)
            {
                var backwardsDict = BiDirectionalBFS.BuildBackwardsMap(i, 64, SmithingMoveHelper.AllMoves, finishingMoves);

                var bestPath = BiDirectionalBFS.SearchForwardsAndMeet(0, i, SmithingMoveHelper.AllMoves, backwardsDict, 64);

                var sum = bestPath?.Sum(entry => entry.PointChange);

                var everythingIsRight = sum == i;

                Assert.Equal(i, sum);
            }
        }
    }
}
