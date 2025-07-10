using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator;

public static class BiDirectionalBFS
{
    public static ConcurrentDictionary<int, List<ISmithingMove>> BuildBackwardsMap(int targetPoints, int maxDepth, List<SmithingMoveTypeEnum> allMoves, List<SmithingMoveTypeEnum> finishingMoveTypes)
    {
        var visited = new ConcurrentDictionary<int, List<ISmithingMove>>();
        visited[targetPoints] = new List<ISmithingMove>();

        var frontier = new ConcurrentQueue<int>();
        frontier.Enqueue(targetPoints);

        for (int depth = 1; depth <= maxDepth; depth++)
        {
            var next = new ConcurrentQueue<int>();
            Parallel.ForEach(frontier, points =>
            {
                var workingPath = visited[points];
                int usedFinishers = workingPath.Count;

                IEnumerable<SmithingMoveTypeEnum> moveTypesToTry;
                if (usedFinishers < finishingMoveTypes.Count)
                {
                    var finishingMove = finishingMoveTypes[finishingMoveTypes.Count - usedFinishers - 1];
                    moveTypesToTry = [finishingMove];
                }
                else
                {
                    moveTypesToTry = allMoves;
                }

                foreach (var moveType in moveTypesToTry)
                {
                    var movesToDo = SmithingMoveHelper.GetAppropriateMoveForMoveEnum(moveType);

                    foreach (var move in movesToDo)
                    {
                        int previous = points - move.PointChange;
                        if (previous <= 0 || previous >= 140) continue;

                        if (visited.TryAdd(previous, new List<ISmithingMove>(workingPath) { move }))
                        {
                            next.Enqueue(previous);
                        }
                    }
                }
            });

            if (next.IsEmpty)
            {
                break;
            }
            frontier = next;
        }

        return visited;
    }

    public static List<ISmithingMove>? SearchForwardsAndMeet(int startPoints, int goalPoints, List<SmithingMoveTypeEnum> allMoves, ConcurrentDictionary<int, List<ISmithingMove>> backwardDict, int maxDepth)
    {
        var visited = new ConcurrentDictionary<int, List<ISmithingMove>>();
        visited[startPoints] = new List<ISmithingMove>();

        var frontier = new ConcurrentQueue<int>();
        frontier.Enqueue(startPoints);

        for (int depth = 1; depth <= maxDepth; depth++)
        {
            var next = new ConcurrentQueue<int>();
            var cts = new CancellationTokenSource();

            var options = new ParallelOptions { CancellationToken = cts.Token };
            List<ISmithingMove>? bestPath = null;

            try
            {
                Parallel.ForEach(frontier, options, points =>
                {
                    if (options.CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var workingPath = visited[points];

                    foreach (var finishingMoveType in allMoves)
                    {
                        var movesToDo = SmithingMoveHelper.GetAppropriateMoveForMoveEnum(finishingMoveType);

                        foreach (var move in movesToDo)
                        {
                            int nextPoints = points + move.PointChange;
                            if (nextPoints <= 0 || nextPoints >= 140) continue;

                            if (backwardDict.TryGetValue(nextPoints, out var backPath))
                            {
                                var fullPath = new List<ISmithingMove>(workingPath) { move };
                                for (int i = backPath.Count - 1; i >= 0; i--)
                                {
                                    fullPath.Add(backPath[i]);
                                }

                                bestPath = fullPath;
                                cts.Cancel();

                                return;
                            }
                            if (depth < maxDepth
                                && visited.TryAdd(nextPoints, new List<ISmithingMove>(workingPath) { move }))
                            {
                                next.Enqueue(nextPoints);
                            }
                        }
                    }

                });
            }
            catch (OperationCanceledException)
            {
                // Will be thrown when we cancel through the CancellationToken
            }

            if (bestPath != null)
            {
                return bestPath;
            }

            if (next.IsEmpty)
                break;

            frontier = next;
        }

        return null;
    }
}
