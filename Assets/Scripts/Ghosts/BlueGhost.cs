using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BlueGhost : Ghost
{
    public override Vector3 Target
    {
        get
        {
            //Get the first red ghost's target position.
            //if it cannnot, then set the position to zero
            Vector3 redGhostPosition = Vector3.zero;
            if (RedGhost.RedGhosts.Count > 0)
            {
                redGhostPosition = RedGhost.RedGhosts[0].transform.position;
            }
            //Get the munchers position
            var muncherPosition = Muncher.MainMuncher.transform.position;

            //Set the taget based on the muncher and the red ghost
            return ((muncherPosition - redGhostPosition) * 2) + redGhostPosition;
        }
    }
}
