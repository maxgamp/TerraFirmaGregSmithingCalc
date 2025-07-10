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
        var visited = new ConcurrentDictionary<(int, int finishersUsed), List<ISmithingMove>>();
        visited[(targetPoints, 0)] = new List<ISmithingMove>();

        var frontier = new ConcurrentQueue<(int points, int finishersUsed)>();
        frontier.Enqueue((targetPoints, 0));

        for (int depth = 1; depth <= maxDepth; depth++)
        {
            var next = new ConcurrentQueue<(int, int)>();
            Parallel.ForEach(frontier, current =>
            {
                var (points, finishersUsed) = current;

                var workingPath = visited[current];

                IEnumerable<SmithingMoveTypeEnum> moveTypesToTry;
                if (finishersUsed < finishingMoveTypes.Count)
                {
                    var finishingMove = finishingMoveTypes[finishingMoveTypes.Count - finishersUsed - 1];
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
                        int nextPoints = points - move.PointChange;
                        if (nextPoints <= -50 || nextPoints >= 200) continue;

                        int nextFinisherUsed = finishersUsed + (finishersUsed < finishingMoveTypes.Count ? 1 : 0);
                        var key = (nextPoints, nextFinisherUsed);

                        var newPath = new List<ISmithingMove>(workingPath) { move };

                        if (visited.TryAdd(key, newPath))
                        {
                            next.Enqueue(key);
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

        var result = new ConcurrentDictionary<int, List<ISmithingMove>>();

        foreach (var kvp in visited)
        {
            var (points, _) = kvp.Key;
            if (result.ContainsKey(points) != true || kvp.Value.Count < result[points].Count)
            {
                result[points] = kvp.Value;
            }

        }

        return result;
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
                            if (nextPoints <= -50 || nextPoints >= 200) continue;

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
