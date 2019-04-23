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
    public string GhostScript;
    public Color color;

    Action IOnLevelLoadWithPost.OnLevelLoad(Vector3Int position)
    {
        var ghostType = Assembly.GetExecutingAssembly().GetType(GhostScript);
        var ghost = GameObject.Instantiate(GameManager.Game.GhostPrefab, position + new Vector3(0.5f, 0.5f), Quaternion.identity).AddComponent(ghostType) as Ghost;
        ghost.GetComponent<SpriteRenderer>().color = color;
        Level.Map.SetTile(position, null);
        return () => ghost.OnGhostSpawn((ghost.transform.position - new Vector3(0.5f, 0.5f)).ToInt());
    }
}
