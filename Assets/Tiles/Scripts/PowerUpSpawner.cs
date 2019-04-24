using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PowerUpSpawner : Tile, IOnLevelLoad
{
    public GameObject PowerUpPrefab; //The prefab to spawn


    //When the level loads
    void IOnLevelLoad.OnLevelLoad(Vector3Int position)
    {
        //Spawn the powerup
        GameObject.Instantiate(PowerUpPrefab, position + new Vector3(0.5f, 0.5f), Quaternion.identity);
        //Remove the spawner
        Level.Map.SetTile(position, null);
    }
}
