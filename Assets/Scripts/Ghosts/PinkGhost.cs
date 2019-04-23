using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PinkGhost : Ghost
{
    private float VectorDistance = 2f;
    public override Vector3 Target
    {
        get
        {
            var muncherDirection = Muncher.MuncherDirection;
            var muncherPosition = Muncher.MainMuncher.transform.position;
            Vector3 finalPosition = muncherPosition;
            switch (muncherDirection)
            {
                case Direction.Up:
                    finalPosition += Vector3.up * VectorDistance;
                    break;
                case Direction.Down:
                    finalPosition += Vector3.down * VectorDistance;
                    break;
                case Direction.Left:
                    finalPosition += Vector3.left * VectorDistance;
                    break;
                case Direction.Right:
                    finalPosition += Vector3.right * VectorDistance;
                    break;
            }
            return finalPosition;
        }
    }
}
