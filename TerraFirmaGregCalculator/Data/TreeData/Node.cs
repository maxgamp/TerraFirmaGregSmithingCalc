using System;
using System.Collections.Generic;
using System.Linq;

namespace TerraFirmaGregCalculator.Data.TreeData;

public class Node
{

    private List<Node>? _children;


    private int _currentPoints;
    public int CurrentPoints { get { return _currentPoints; } }

    private ISmithingMove? _move;
    public ISmithingMove? SmithingMove { get { return _move; } }

    public Node(int goalPoints)
    {
        _currentPoints = goalPoints;
    }

    public Node(ISmithingMove move, int currentPoints)
    {
        _move = move;
        _currentPoints = currentPoints + move.PointChange;
    }

    public bool NextLayerDefinedMoveAllHits(SmithingMoveTypeEnum moveType)
    {
        var hitMoveArr = new SmithingMoveHitTypeEnum[3];
        hitMoveArr[0] = SmithingMoveHitTypeEnum.Light;
        hitMoveArr[1] = SmithingMoveHitTypeEnum.Medium;
        hitMoveArr[2] = SmithingMoveHitTypeEnum.Hard;

        if (_children == null)
        {
            _children = new List<Node>();
            var success = false;
            for (int i = 0; i <= 2; i++)
            {
                var newNode = MakeNewNode(moveType, hitMoveArr[i]);
                if (newNode != null)
                {
                    _children.Add(newNode);
                    success = newNode.ReachedGoalPoints();
                }
                else
                {
                    Console.WriteLine("Error! Tried to add null to _children of node!");
                }
            }

            return success;
        }
        else
        {
            Node? doneNode = null;
            foreach (var node in _children)
            {
                var success = node.NextLayerDefinedMoveAllHits(moveType);
                if (success)
                    doneNode = node;
            }
            if (doneNode != null)
                return true;
        }
        return false;
    }

    public bool NextLayerDefinedMove(SmithingMoveTypeEnum moveType)
    {
        if (_children == null)
        {
            _children = new List<Node>();
            var newNode = MakeNewNode(moveType);
            if (newNode != null)
            {
                _children.Add(newNode);
                return newNode.ReachedGoalPoints();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        else
        {
            Node? doneNode = null;
            foreach (var node in _children)
            {
                var success = node.NextLayerDefinedMove(moveType);
                if (success)
                    doneNode = node;
            }
            if (doneNode != null)
                return true;
        }
        return false;
    }

    public bool NextLayerNBestMoves(int timeToLive, int doNBestMoves)
    {
        if (timeToLive == 0)
            return false;
        if (_children == null)
        {
            _children = new List<Node>();
            var sortedList = new List<Enum>(SmithingMoveHelper.PossibleMoves);

            sortedList.Sort((first, second) =>
            {
                var firstTotalPoints = _currentPoints + first.GetMovePointValue();
                var secondTotalPoints = _currentPoints + second.GetMovePointValue();

                var firstFitness = Math.Abs(0 - firstTotalPoints);
                var secondFitness = Math.Abs(0 - secondTotalPoints);

                return firstFitness.CompareTo(secondFitness);
            });

            var movesToDo = new List<Enum>(sortedList.Take(doNBestMoves));

            foreach (var move in movesToDo)
            {
                var newNode = MakeNewNode(move);

                ArgumentNullException.ThrowIfNull(newNode);

                _children.Add(newNode);
            }
        }
        else
        {
            bool success = false;
            foreach (var node in _children)
            {
                success = node.NextLayerNBestMoves(--timeToLive, doNBestMoves);
            }
            if (success)
                return true;
        }
        if (_children?.Any(child => child.ReachedGoalPoints()) == true)
            return true;
        return false;
    }

    public List<Node> GetTree()
    {
        var treeList = new List<Node>();
        treeList.Add(this);

        if (_children != null)
        {
            foreach (var child in _children)
            {
                treeList.AddRange(child.GetTree());
            }
        }

        return treeList;
    }

    public List<MoveListObject>? GetCorrectPaths()
    {
        if (_children == null)
        {
            if (ReachedGoalPoints())
            {
                return [new(this)];
            }
            else
            {
                return null;
            }
        }

        var outputList = new List<MoveListObject>();


        foreach (var child in _children)
        {
            var childList = child.GetCorrectPaths();
            if (childList is not null)
            {
                foreach (var childChild in childList)
                {
                    childChild.AddMove(this);
                    outputList.Add(childChild);
                }
            }
        }

        return outputList;
    }

    public override string ToString()
    {
        var output = _move?.PointChange + " ";

        if (_children == null)
        {
            return "Points: " + _currentPoints + "\n";
        }

        foreach (var child in _children)
        {
            output += "- " + child.ToString();
        }

        return output;
    }

    private bool ReachedGoalPoints()
    {
        return _currentPoints == 0;
    }

    private Node? MakeNewNode(SmithingMoveTypeEnum move, SmithingMoveHitTypeEnum? hitMove = null)
    {
        return move switch
        {
            SmithingMoveTypeEnum.Punch => new Node(new PositiveSmithingMove(move), _currentPoints),
            SmithingMoveTypeEnum.Bend => new Node(new PositiveSmithingMove(move), _currentPoints),
            SmithingMoveTypeEnum.Upset => new Node(new PositiveSmithingMove(move), _currentPoints),
            SmithingMoveTypeEnum.Shrink => new Node(new PositiveSmithingMove(move), _currentPoints),
            SmithingMoveTypeEnum.Hit => new Node(new NegativeSmithingMoveHit(hitMove), _currentPoints),
            SmithingMoveTypeEnum.Draw => new Node(new NegativeSmithingMove(move), _currentPoints),
            _ => null,
        };
    }

    private Node? MakeNewNode(Enum inputEnum)
    {
        if (inputEnum is SmithingMoveHitTypeEnum moveHitTypeEnum)
        {
            return MakeNewNode(SmithingMoveTypeEnum.Hit, moveHitTypeEnum);
        }
        else
        if (inputEnum is SmithingMoveTypeEnum moveTypeEnum)
        {
            return MakeNewNode(moveTypeEnum);
        }

        return null;
    }
}
