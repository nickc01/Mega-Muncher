using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OrangeGhost : Ghost
{
    private float MinDistance = 6f; //The minimum distance needed to change the state to random
    public override Vector3 Target
    {
        get
        {
            //Get the munchers position
            var muncherPosition = Muncher.MainMuncher.transform.position;
            //Set the ghost state depending on how far away the ghost is from the muncher
            if (Vector3.Distance(muncherPosition,transform.position) > MinDistance)
            {
                CurrentState = TargetingState.Targeting;
            }
            else
            {
                CurrentState = TargetingState.Random;
            }
            return muncherPosition;
        }
    }
}
