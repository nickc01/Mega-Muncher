using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RedGhost : Ghost
{
    //A list if red ghosts in the game
    private static List<RedGhost> redGhosts = new List<RedGhost>();
    //A public interface for accessing all the red ghosts
    public static ReadOnlyCollection<RedGhost> RedGhosts => redGhosts.AsReadOnly();

    //When the ghost spawns in
    public override void OnGhostSpawn(Vector3Int spawnPoint)
    {
        //Add this ghost to the ghost list
        redGhosts.Add(this);
        //Run the base method
        base.OnGhostSpawn(spawnPoint);
    }

    //When the level ends
    protected override void OnLevelEnd()
    {
        //Remove this ghost from the ghost list
        redGhosts.Remove(this);
        //Run the base method
        base.OnLevelEnd();
    }
}
