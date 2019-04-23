using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RedGhost : Ghost
{
    private static List<RedGhost> redGhosts = new List<RedGhost>();
    public static ReadOnlyCollection<RedGhost> RedGhosts => redGhosts.AsReadOnly();
    public override void OnGhostSpawn(Vector3Int spawnPoint)
    {
        redGhosts.Add(this);
        base.OnGhostSpawn(spawnPoint);
    }

    protected override void OnLevelEnd()
    {
        redGhosts.Remove(this);
        base.OnLevelEnd();
    }
}
