using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GhostSmart : Ghost
{
    //Uses a pathfinding algorithm to more accurately find it's destination
    protected override Direction PickDirection(Vector3Int PreviousPosition, Vector3Int CurrentPosition, Direction PreviousDirection)
    {
        return base.PickDirection(PreviousPosition, CurrentPosition, PreviousDirection);
    }
}