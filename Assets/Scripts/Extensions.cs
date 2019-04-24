using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Direction;

namespace Extensions
{
    public static class Extensions
    {
        //An extension for converting a Vector3 to a Vector3Int
        public static Vector3Int ToInt(this Vector3 vector)
        {
            return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        }
        //Returns true if the directions are opposites, and false otherwise
        public static bool IsOppositeTo(this Direction A, Direction B)
        {
            return B == Opposite(A);
        }
        //Returns the opposite of the current direction
        public static Direction Opposite(this Direction A)
        {
            switch (A)
            {
                case Up:
                    return Down;
                case Down:
                    return Up;
                case Left:
                    return Right;
                case Right:
                    return Left;
                default:
                    return default;
            }
        }
        //Converts the direction into a unit vector
        public static Vector3Int ToVector(this Direction A)
        {
            switch (A)
            {
                case Up:
                    return Vector3Int.up;
                case Down:
                    return Vector3Int.down;
                case Left:
                    return Vector3Int.left;
                case Right:
                    return Vector3Int.right;
                case None:
                    return Vector3Int.zero;
            }
            return default;
        }
    }
}
