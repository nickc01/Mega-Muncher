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
            Vector3 redGhostPosition = Vector3.zero;
            if (RedGhost.RedGhosts.Count > 0)
            {
                redGhostPosition = RedGhost.RedGhosts[0].transform.position;
            }
            var muncherPosition = Muncher.MainMuncher.transform.position;

            return ((muncherPosition - redGhostPosition) * 2) + redGhostPosition;
        }
    }
}
