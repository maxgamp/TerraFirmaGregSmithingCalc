
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TerraFirmaGregCalculator.Data;
using TerraFirmaGregCalculator.Data.TreeData;

public class Program
{

    private ISmithingMove[] MoveArr;

    public Program()
    {
        MoveArr = [
            new PositiveSmithingMove(SmithingMoveTypeEnum.Punch, 2),
            new PositiveSmithingMove(SmithingMoveTypeEnum.Bend, 7),
            new PositiveSmithingMove(SmithingMoveTypeEnum.Upset, 13),
            new PositiveSmithingMove(SmithingMoveTypeEnum.Shrink, 16),
            new NegativeSmithingMoveHit( SmithingMoveHitTypeEnum.Light, 3),
            new NegativeSmithingMoveHit(SmithingMoveHitTypeEnum.Medium, 6),
            new NegativeSmithingMoveHit(SmithingMoveHitTypeEnum.Hard, 9),
            new NegativeSmithingMove(SmithingMoveTypeEnum.Draw, 15)
        ];
    }

    public static void Main(string[] args)
    {
        do
        {
            CalculateSmithingOrder();
            Console.WriteLine("Press any key to clear the screen and begin anew.");
            Console.ReadKey(true);
            Console.Clear();
        } while (true);
    }

    private static void CalculateSmithingOrder()
    {
        bool success = false;
        int goalSum = 0;
        List<SmithingMoveTypeEnum> finishingMoves = new List<SmithingMoveTypeEnum>();

        do
        {
            Console.WriteLine("Input goal sum: ");

            var numInput = Console.ReadLine();

            success = int.TryParse(numInput, out goalSum);

            if (goalSum <= 0 || goalSum >= 100)
            {
                Console.WriteLine("Value has to be greater than 0 and lower than 100");
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
                [E]xit

                Selected options:
                {selectedMoves}
                """);

            var choice = Console.ReadKey(true);

            if (choice.Key == ConsoleKey.E)
            {
                break;
            }

            var smithingMove = ChooseSmithingMove(choice);

            if (smithingMove != null)
            {
                finishingMoves.Add((SmithingMoveTypeEnum)smithingMove);
            }
            else
            {
                Console.WriteLine($"Could not find character {choice.Key} in switch expression.");
                Console.ReadKey(true);
            }
        } while (success != true);


        var rootNode = new Node(-goalSum);

        foreach (var move in finishingMoves)
        {
            if (move == SmithingMoveTypeEnum.Hit)
            {
                rootNode.NextLayerDefinedMoveAllHits(move);
            }
            else
            {
                rootNode.NextLayerDefinedMove(move);
            }
        }

        long lastTimestamp = Stopwatch.GetTimestamp();
        var ttl = 4;
        do
        {
            success = rootNode.NextLayerNBestMoves(ttl, 2);
            ttl++;

            Console.WriteLine("TTL: " + ttl + "\nTime since in Seconds: " + Stopwatch.GetElapsedTime(lastTimestamp).TotalSeconds);
            lastTimestamp = Stopwatch.GetTimestamp();
        } while (success != true);

        var wholeTree = rootNode.GetTree();

        var correctPaths = rootNode.GetCorrectPaths();

        Console.WriteLine($"TargetScore: {goalSum}.\n");
        ArgumentNullException.ThrowIfNull(correctPaths);

        string output = "| Points | MoveName    | Change |\n";

        correctPaths.Sort((first, second) => first.MoveCount.CompareTo(second.MoveCount));

        var fastestApproach = correctPaths.First();

        foreach (var node in fastestApproach.MoveList)
        {
            output +=
            $"|  {0 - node.CurrentPoints,-4:D3}  | {node.SmithingMove?.GetSmithingMoveName() ?? "Done",-11} |   {node.SmithingMove?.PointChange,-3:D2}  |\n";
        }

        Console.WriteLine(output);
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
}