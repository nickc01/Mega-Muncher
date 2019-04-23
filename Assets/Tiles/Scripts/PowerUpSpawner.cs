using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PowerUpSpawner : Tile, IOnLevelLoad
{
    public GameObject PowerUpPrefab;

    void IOnLevelLoad.OnLevelLoad(Vector3Int position)
    {
        var newPowerUp = GameObject.Instantiate(PowerUpPrefab, position + new Vector3(0.5f, 0.5f), Quaternion.identity);
        Level.Map.SetTile(position, null);
    }
}
