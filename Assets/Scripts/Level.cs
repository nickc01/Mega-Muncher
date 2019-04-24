using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Reflection;
using static GameManager;
using Extensions;
using System;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public static Level Singleton { get; set; } //The singleton for the level loader
    public static Tilemap Map => Singleton.gameMap; //The public interface for accessing the loaded level map
    [SerializeField] Tilemap gameMap; //The loaded level map
    [SerializeField] AudioClip levelMusic; //The music for this specific level
    public Color LevelColor; //The primary color for this specific level
    public Color SecondaryLevelColor; //The secondary skybox color for this specific level
    public static Vector3Int SpawnPoint { get; private set; } //The spawnpoint for the muncher
    private static bool SpawnPointSet = false; //Is set to true if the spawnpoint is set
    public static BoundsInt Boundaries => Singleton.gameMap.cellBounds; //The boundaries of the map

    private static Dictionary<int, List<Teleporter>> TeleporterLists = new Dictionary<int, List<Teleporter>>(); //A list of all the teleporters in the map
    public static AudioClip LevelMusic => Singleton.levelMusic;  //The public interface for accessing the level music

    void Start()
    {
        //Set the background colors
        BackgroundImages.SetColor(LevelColor, SecondaryLevelColor);
        //A list of functions called when the loading is finished
        Action postLoads = null;
        //Update the singleton
        Singleton = this;
        //Reset the spawnpoint
        SpawnPointSet = false;
        //Compress the map boundaries
        gameMap.CompressBounds();
        var bounds = gameMap.cellBounds;
        //Loop over every single tile in the map
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                var tile = gameMap.GetTile(pos);
                if (tile != null)
                {
                    //If the tile is a spawnpoint for the muncher
                    if (tile.name == "SpawnPoint")
                    {
                        //Update the current spawnpoint if it is not already set
                        if (SpawnPointSet == false)
                        {
                            SpawnPointSet = true;
                            SpawnPoint = pos;
                        }
                        gameMap.SetTile(pos, null);
                    }
                    //If the tile is a pellet tile
                    if (tile.name == "PelletTile")
                    {
                        //Spawn pellet gameobjects at that tile
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
                    //If the tile has loading code
                    if (tile is IOnLevelLoad loader)
                    {
                        //Run the loading code
                        loader.OnLevelLoad(pos);
                    }
                    //If the tile has both loading code and post-loading code
                    else if (tile is IOnLevelLoadWithPost post)
                    {
                        //Run the loading code and add the post-loading code to the postLoads event handler for later
                        postLoads += post.OnLevelLoad(pos);
                    }
                }
            }
        }
        //Spawn the muncher at the spawnpoint and call it's onspawn function
        GameObject.Instantiate(Game.MuncherPrefab, SpawnPoint + new Vector3(0.5f, 0.5f), Quaternion.identity).GetComponent<Muncher>().OnMuncherSpawn();
        //Run the post-load code
        postLoads?.Invoke();
    }
}
