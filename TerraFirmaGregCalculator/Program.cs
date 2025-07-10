using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TerraFirmaGregCalculator.Data;

namespace TerraFirmaGregCalculator;

public class Program
{
    private static int _maxDepth;
    private static bool _assumeStartingPointsZero;

    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: true);

        IConfiguration config = builder.Build();

        var calculatorSettings = new CalculatorSettings();

        config.GetSection(nameof(CalculatorSettings)).Bind(calculatorSettings);

        _maxDepth = calculatorSettings?.MaxDepth ?? 64;
        _assumeStartingPointsZero = calculatorSettings?.AssumeStartingPointsZero ?? true;

        do
        {
            HandleInputs();
            Console.WriteLine("Press any key to clear the screen and begin anew.");
            Console.ReadKey(true);
            Console.Clear();
        } while (true);
    }

    private static void HandleInputs()
    {
        bool success = false;
        int targetPoints = 0;
        int startPoints = 0;
        List<SmithingMoveTypeEnum> finishingMoves = new List<SmithingMoveTypeEnum>();

        if (_assumeStartingPointsZero != true)
        {
            do
            {
                Console.WriteLine("Input starting points: ");
                var numInput = Console.ReadLine();

                success = int.TryParse(numInput, out startPoints);

                if (startPoints < 0 || targetPoints > 150)
                {
                    Console.WriteLine("Value has to be between 0 and 150.");
                    success = false;
                }

            } while (success != true);
        }

        do
        {
            Console.WriteLine("Input target points: ");

            var numInput = Console.ReadLine();

            success = int.TryParse(numInput, out targetPoints);

            if (targetPoints <= 0 || targetPoints >= 150)
            {
                Console.WriteLine("Value has to be greater than 0 and lower than 150");
                success = false;
            }
        } while (success != true);

        success = false;

        do
        {
            Console.Clear();
            var position = finishingMoves.Count == 0 ? "last" : finishingMoves.Count == 1 ? "second last" : "third last";
            var selectedMoves = "- " + string.Join("\n- ", finishingMoves.Select(move => move));
            Console.WriteLine($"""
            Choose the the {position} move:
            [P]unch
            [B]end
            [U]pset
            [S]hrink
            [H]it
            [D]raw
            ---------------------------------
            [R]emove last picked option
            [C]alculate

            Selected options:
            {selectedMoves}
            """);

            var choice = Console.ReadKey(true);

            if (choice.Key == ConsoleKey.C)
            {
                break;
            }
            if (choice.Key == ConsoleKey.R)
            {
                finishingMoves.RemoveAt(finishingMoves.Count - 1);
                continue;
            }

            var smithingMove = ChooseSmithingMove(choice);

            if (smithingMove != null)
            {
                finishingMoves.Add((SmithingMoveTypeEnum)smithingMove);
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"Could not find character {choice.Key} in switch expression.\n[Press any key to continue]");
                Console.ReadKey(true);
            }
        } while (success != true);

        List<ISmithingMove>? bestPath = CalculateSmithingOrderBiDirectional(startPoints, targetPoints, finishingMoves, SmithingMoveHelper.AllMoves, _maxDepth);

        if (bestPath == null || bestPath.Count == 0)
        {
            Console.WriteLine("Couldn't find a path to target points, try again with increased maxPaths.");
            return;
        }

        var optimizedList = FixTreeGoingNegative(bestPath, finishingMoves.Count);

        var simplifiedList = SimplifyTree(optimizedList, finishingMoves.Count);

        Console.WriteLine($"TargetScore: {targetPoints}.\n");

        Console.WriteLine("SimplifiedTree/OutputCombined:\n" + MakeTreeOutputCombined(simplifiedList) + "\n");
    }

    public static List<ISmithingMove>? CalculateSmithingOrderBiDirectional(int startPoints, int goalPoints, List<SmithingMoveTypeEnum> finishingMoves, List<SmithingMoveTypeEnum> allMoves, int maxDepth)
    {
        var backwardsDict = BiDirectionalBFS.BuildBackwardsMap(goalPoints, maxDepth, allMoves, finishingMoves);

        var bestPath = BiDirectionalBFS.SearchForwardsAndMeet(startPoints, goalPoints, allMoves, backwardsDict, maxDepth);

        return bestPath;
    }

    private static SmithingMoveTypeEnum? ChooseSmithingMove(ConsoleKeyInfo key) => key.Key switch
    {
        ConsoleKey.P => SmithingMoveTypeEnum.Punch,
        ConsoleKey.B => SmithingMoveTypeEnum.Bend,
        ConsoleKey.U => SmithingMoveTypeEnum.Upset,
        ConsoleKey.S => SmithingMoveTypeEnum.Shrink,
        ConsoleKey.H => SmithingMoveTypeEnum.Hit,
        ConsoleKey.D => SmithingMoveTypeEnum.Draw,
        _ => null,
    };


    public static List<ISmithingMove> FixTreeGoingNegative(List<ISmithingMove> originalList, int finishingMovesCount)
    {
        var optimizedList = new List<ISmithingMove>();
        List<ISmithingMove> movesToDo = new();

        int pointCount = 0;

        for (int i = 0; i < originalList.Count - finishingMovesCount; i++)
        {
            var curMove = originalList[i];
            if (curMove is null)
            {
                Console.WriteLine(new ArgumentNullException(nameof(curMove)));
                Console.WriteLine("Ran into an issue optimizing, displaying raw output.\n");
                return originalList;
            }

            if (pointCount + curMove.PointChange >= 0)
            {
                optimizedList.Add(curMove);
                pointCount += curMove.PointChange;
            }
            else
            {
                movesToDo.Add(originalList[i]!);
            }
        }

        var tmpPointCount = pointCount;
        for (int i = 0; i < movesToDo.Count; i++)
        {
            if (tmpPointCount + movesToDo[i].PointChange >= 0)
            {
                optimizedList.Add(movesToDo[i]);
                tmpPointCount += movesToDo[i].PointChange;
            }
            else
            {
                Console.WriteLine(new ArgumentOutOfRangeException(nameof(movesToDo)));
                Console.WriteLine("Ran into an issue optimizing, displaying raw output.\n");
                return originalList;
            }
        }

        for (int i = finishingMovesCount; i > 0; i--)
        {
            var nthLastMove = originalList[^i];
            optimizedList.Add(nthLastMove);
        }

        return optimizedList;
    }


    public static List<ISmithingMove> SimplifyTree(List<ISmithingMove> treeList, int finishingMovesCount)
    {
        Dictionary<Enum, int> moveCountDict = new(SmithingMoveHelper.PossibleMoves.ToDictionary(move => move, move => 0));

        var subListToExamine = treeList[0..^(finishingMovesCount - 1)]; // Look at the last finishing Move to determine if we can move other moves to the end

        foreach (var moveKey in moveCountDict.Keys)
        {
            int count = 0;
            if (moveKey is SmithingMoveTypeEnum moveType)
            {
                count = subListToExamine.Where(treeMove => treeMove.MoveType == moveType).Count();
            }
            else if (moveKey is SmithingMoveHitTypeEnum moveTypeHit)
            {
                foreach (var move in subListToExamine)
                {
                    if (move is NegativeSmithingMoveHit smithingMoveHit && moveTypeHit == smithingMoveHit.HitType)
                    {
                        count++;
                    }
                }
            }

            moveCountDict[moveKey] = count;
        }

        var outputList = new List<ISmithingMove>();
        var distinctList = subListToExamine.Distinct(new ISmithingMoveComparer()).ToList();

        int GetCount(ISmithingMove move)
        {
            if (move is NegativeSmithingMoveHit moveHit)
            {
                return moveCountDict[moveHit.HitType!];
            }
            else if (move.MoveType is SmithingMoveTypeEnum moveType)
            {
                return moveCountDict[moveType];
            }
            return 0;
        }

        var comparer = new ISmithingMoveComparer();
        foreach (var move in distinctList)
        {
            if (comparer.Equals(move, subListToExamine.Last()))
            {
                continue;
            }

            ArgumentNullException.ThrowIfNull(move);

            int count = GetCount(move);

            for (int i = 0; i < count; i++)
            {
                outputList.Add(move);
            }
        }

        var finalMove = subListToExamine.Last();
        ArgumentNullException.ThrowIfNull(finalMove);
        for (int i = 0; i < GetCount(finalMove); i++)
        {
            outputList.Add(finalMove);
        }

        for (int i = finishingMovesCount - 1; i >= 1; i--)
        {
            outputList.Add(treeList[^i]);
        }

        return outputList;
    }


    public static string MakeTreeOutputSimple(List<ISmithingMove> treeList)
    {
        var output = "| Points | MoveName    | Change |\n";

        int points = 0;
        foreach (var move in treeList)
        {
            points += move.PointChange;
            output +=
            $"|  {points,-4:D3}  | {move?.GetSmithingMoveName() ?? "Done",-11} |   {(move?.PointChange >= 0 ? "+" : "") + move?.PointChange.ToString(),-3:D2}  |\n";
        }
        output += "---------------------------------";

        return output;
    }

    public static string MakeTreeOutputCombined(List<ISmithingMove> treeList)
    {
        var sb = new StringBuilder();

        sb.AppendLine("| Count | MoveName    | Change | Points |");

        int points = 0;
        ISmithingMove? previousMove = null;
        int moveRepeats = 0;
        for (int i = 0; i < treeList.Count; i++)
        {
            var move = treeList[i];
            if (previousMove != null)
            {
                if (i + 1 < treeList.Count
                    && move.MoveType == treeList[i + 1]?.MoveType)
                {
                    moveRepeats++;
                }
                else
                {
                    points += move.PointChange * moveRepeats;
                    AddTreeLineToStringBuilder(sb, points, move, moveRepeats);

                    moveRepeats = 0;
                    previousMove = null;
                }
            }
            else
            {
                if (i + 1 < treeList.Count
                    && move.MoveType == treeList[i + 1]?.MoveType)
                {
                    previousMove = move;
                    moveRepeats = 2;
                    continue;
                }
                else
                {
                    points += move.PointChange;
                    AddTreeLineToStringBuilder(sb, points, move, 1);
                }
            }
        }
        sb.AppendLine("-----------------------------------------");

        return sb.ToString();
    }

    public static void AddTreeLineToStringBuilder(StringBuilder sb, int points, ISmithingMove move, int count)
    {
        sb.AppendLine($"|  {count,-3:D2}  | {move?.GetSmithingMoveName() ?? "Done",-11} |  {(move?.PointChange >= 0 ? "+" : "") + move?.PointChange.ToString(),-3:D2}   |  {points,-4:D3}  |");
    }
}