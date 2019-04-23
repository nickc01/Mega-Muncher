using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TeleporterTile : Tile, IOnLevelLoad
{
    public int TeleportID = 0;

    void IOnLevelLoad.OnLevelLoad(Vector3Int position)
    {
        var newTeleporter = GameObject.Instantiate(GameManager.Game.TeleporterPrefab, position + new Vector3(0.5f, 0.5f), Quaternion.identity).GetComponent<Teleporter>();
        newTeleporter.Position = position;
        newTeleporter.LinkID = TeleportID;
        Level.Map.SetTile(position, null);
        Teleporter.Teleporters.Add(newTeleporter);
    }
}
