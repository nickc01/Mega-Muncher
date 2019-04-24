using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TeleporterTile : Tile, IOnLevelLoad
{
    public int TeleportID = 0; //The LinkID of the spawned teleporter

    //When the level loads
    void IOnLevelLoad.OnLevelLoad(Vector3Int position)
    {
        //Spawn the teleporter
        var newTeleporter = GameObject.Instantiate(GameManager.Game.TeleporterPrefab, position + new Vector3(0.5f, 0.5f), Quaternion.identity).GetComponent<Teleporter>();
        //Set the teleporter's position
        newTeleporter.Position = position;
        //Set the teleporter's link ID
        newTeleporter.LinkID = TeleportID;
        //Remove the teleporter spawner
        Level.Map.SetTile(position, null);
        //Add the teleporter to the list of teleporters in the level
        Teleporter.Teleporters.Add(newTeleporter);
    }
}
