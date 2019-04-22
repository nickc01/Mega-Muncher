using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Game { get; private set; }
    [Header("Prefabs")]
    public GameObject PelletPrefab;
    public GameObject MuncherPrefab;
    public GameObject GhostPrefab;


    //Called when the player wins the game
    public static event Action OnWin;
    //Called when the player looses a life
    public static event Action OnLose;
    //Called on either winning the game or loosing a life
    public static event Action OnGameEnd;
    //Called when the level has finished and the level is closing
    public static event Action OnLevelEnd;
    // Start is called before the first frame update
    void Start()
    {
        Game = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Win()
    {
        OnWin?.Invoke();
        OnGameEnd?.Invoke();
    }

    public static void Lose()
    {
        OnLose?.Invoke();
        OnGameEnd?.Invoke();
    }
}
