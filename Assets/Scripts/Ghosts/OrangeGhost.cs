using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OrangeGhost : Ghost
{
    private float MinDistance = 6f;
    public override Vector3 Target
    {
        get
        {
            var muncherPosition = Muncher.MainMuncher.transform.position;
            if (Vector3.Distance(muncherPosition,transform.position) > MinDistance)
            {
                CurrentState = GhostState.Targeting;
            }
            else
            {
                CurrentState = GhostState.Random;
            }
            return muncherPosition;
        }
    }
}
