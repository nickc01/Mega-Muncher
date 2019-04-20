using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GameManager;

public class Level : MonoBehaviour
{
    // Start is called before the first frame update
    static Level Singleton { get; set; }
    public static Tilemap Map => Singleton.gameMap;
    [SerializeField] Tilemap gameMap;
    public static Vector3Int SpawnPoint { get; private set; }
    private static bool SpawnPointSet = false;
    private static List<Vector3Int> ghostSpawns = new List<Vector3Int>();
    public static ReadOnlyCollection<Vector3Int> GhostSpawns => ghostSpawns.AsReadOnly();
    public static BoundsInt Boundaries => Singleton.gameMap.cellBounds;
    void Start()
    {
        Singleton = this;
        SpawnPointSet = false;
        ghostSpawns.Clear();
        gameMap.CompressBounds();
        var bounds = gameMap.cellBounds;
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                var tile = gameMap.GetTile(pos);
                if (tile != null)
                {
                    if (tile.name == "SpawnPoint")
                    {
                        if (SpawnPointSet == false)
                        {
                            SpawnPointSet = true;
                            SpawnPoint = pos;
                        }
                        gameMap.SetTile(pos, null);
                    }
                    if (tile.name == "PelletTile")
                    {
                        gameMap.SetTile(pos, null);
                        GameObject.Instantiate(Game.PelletPrefab, pos + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        if (!gameMap.HasTile(pos + Vector3Int.down))
                        {
                            GameObject.Instantiate(Game.PelletPrefab, pos + new Vector3(0.5f, 0), Quaternion.identity);
                        }
                        if (!gameMap.HasTile(pos + Vector3Int.left))
                        {
                            GameObject.Instantiate(Game.PelletPrefab, pos + new Vector3(0f,0.5f), Quaternion.identity);
                        }
                    }
                    if (tile is GhostSpawner)
                    {
                        ghostSpawns.Add(pos);
                        gameMap.SetTile(pos, null);
                    }
                }
            }
        }
        GameObject.Instantiate(Game.MuncherPrefab, SpawnPoint + new Vector3(0.5f, 0.5f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
