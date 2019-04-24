using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Reflection;
using Extensions;
using System;

[CreateAssetMenu]
public class GhostSpawner : Tile, IOnLevelLoadWithPost
{
    public string GhostScript; //The script that will make the ghost unique
    public Color color; //The color of the ghost to spawn

    //When the level loads
    Action IOnLevelLoadWithPost.OnLevelLoad(Vector3Int position)
    {
        //Get the ghost script
        var ghostType = Assembly.GetExecutingAssembly().GetType(GhostScript);
        //Spawn the ghost with the ghost script added to it
        var ghost = GameObject.Instantiate(GameManager.Game.GhostPrefab, position + new Vector3(0.5f, 0.5f), Quaternion.identity).AddComponent(ghostType) as Ghost;
        //Set the ghost's color
        ghost.GetComponent<SpriteRenderer>().color = color;
        //Remove the ghost spawner tile
        Level.Map.SetTile(position, null);
        //When the level is done loaded, run the OnSpawn function of the ghost
        return () => ghost.OnGhostSpawn((ghost.transform.position - new Vector3(0.5f, 0.5f)).ToInt());
    }
}
