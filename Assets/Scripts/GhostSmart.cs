using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Direction;

public class PathBlock
{
    Vector3Int Position;
    PathBlock Previous;
    bool TeleportTo = false;

    public PathBlock(Vector3Int position, PathBlock previous,bool teleportTo = false)
    {
        Position = position;
        Previous = previous;
        TeleportTo = teleportTo;
    }

    public bool EqualTo(PathBlock b)
    {
        return b.Position == Position && Previous == b.Previous;
    }
}

public abstract class GhostSmart : Ghost
{
    //Uses a pathfinding algorithm to more accurately find it's destination
    protected override Direction PickDirection(Vector3Int PreviousPosition, Vector3Int CurrentPosition, Direction PreviousDirection)
    {
        /*List<PathBlock> ProcessingList = new List<PathBlock>();
        List<PathBlock> FinishedList = new List<PathBlock>();
        var topBlock = new PathBlock(CurrentPosition, null);
        FinishedList.Add(topBlock);
        List<(Vector3Int, bool)> Neighbors = null;
        if (AllowReversing)
        {
            Neighbors = GetNeighbors(CurrentPosition);
        }
        else
        {
            Neighbors = GetNeighbors(CurrentPosition,OppositeOf(PreviousDirection));
        }
        foreach (var neighbor in Neighbors)
        {
            ProcessingList.Add(new PathBlock(neighbor.Item1,topBlock,neighbor.Item2));
        }

        while (ProcessingList.Count != 0)
        {
            List<PathBlock> NextProcessingList = new List<PathBlock>();
            foreach (var item in ProcessingList)
            {
                
            }
        }*/






        return base.PickDirection(PreviousPosition, CurrentPosition, PreviousDirection);
    }

    private List<(Vector3Int, bool)> GetNeighbors(Vector3Int position,Direction excludedDirection = None)
    {
        List<(Vector3Int,bool)> neighbors = new List<(Vector3Int,bool)>();
        if (!Level.Map.HasTile(position + Vector3Int.up) && excludedDirection != Up)
        {
            neighbors.Add((position + Vector3Int.up,false));
        }
        if (!Level.Map.HasTile(position + Vector3Int.down) && excludedDirection != Down)
        {
            neighbors.Add((position + Vector3Int.down, false));
        }
        if (!Level.Map.HasTile(position + Vector3Int.left) && excludedDirection != Left)
        {
            neighbors.Add((position + Vector3Int.left, false));
        }
        if (!Level.Map.HasTile(position + Vector3Int.right) && excludedDirection != Right)
        {
            neighbors.Add((position + Vector3Int.right, false));
        }
        var teleporter = Teleporter.GetTeleporter(position);
        if (teleporter != null)
        {
            neighbors.Add((teleporter.LinkedTeleporter.Position,true));
        }
        return neighbors;
    }
}