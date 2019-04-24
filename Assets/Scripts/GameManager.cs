using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum GameState
{
    Win,
    Lose,
    Menu,
    Playing,
    Loading
}

public class GameManager : MonoBehaviour
{
    [SerializeField] int totalLevels = 1;
    public static int Levels => Game.totalLevels;
    private static GameState gameStateInternal = GameState.Menu;
    public static GameState CurrentGameState
    {
        get => gameStateInternal;
        set
        {
            if (gameStateInternal == value)
            {
                return;
            }
            gameStateInternal = value;
            switch (value)
            {
                case GameState.Win: //Wins the game
                    Win();
                    break;
                case GameState.Lose: //Loses the game
                    Lose();
                    break;
                case GameState.Menu: //Goes to the main menu
                    MainMenu();
                    break;
                case GameState.Playing: //Sets the game into motion
                    PlayLevel();
                    break;
                case GameState.Loading: //Starting the game with the selected level
                    LoadLevel();
                    break;
            }
        }
    }
    public static int SelectedLevel = 0;
    public static GameManager Game { get; private set; }
    [Header("Prefabs")]
    public GameObject PelletPrefab;
    public GameObject MuncherPrefab;
    public GameObject GhostPrefab;
    public GameObject TeleporterPrefab;
    public GameObject ExplosionPrefab;
    [Header("Sounds")]
    public AudioClip PelletSound;
    public AudioClip PowerUpSound;
    public AudioClip EatGhostSound;

    private AudioSource audio;


    //Called when the player wins the game
    public static event Action OnWin;
    //Called when the player looses a life
    public static event Action OnLose;
    //Called on either winning the game or loosing a life
    public static event Action OnGamePause;
    //Called to set everything in motion when the game starts
    public static event Action OnGameStart;
    //Called to clean up the level before it is unloaded
    public static event Action OnLevelEnd;
    //Called to reset the level's objects to their starting state
    public static event Action OnLevelReset;
    // Start is called before the first frame update
    void Start()
    {
        Game = this;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async static void Win()
    {
        OnGamePause?.Invoke();
        OnWin?.Invoke();
        Game.audio.Pause();
        await Task.Run(() => Thread.Sleep(1000));
        UIManager.SetState("Win Screen");
    }

    private async static void Lose()
    {
        OnGamePause?.Invoke();
        OnLose?.Invoke();
        Game.audio.Pause();
        await Task.Run(() => Thread.Sleep(1000));
        Muncher.Lives--;
        if (Muncher.Lives == 0)
        {
            UIManager.SetState("Lose Screen");
            Game.audio.Stop();
        }
        else
        {
            OnLevelReset?.Invoke();
            UIManager.SetState("Ready");
            await Task.Run(() => Thread.Sleep(2000));
            UIManager.SetState("Game");
            CurrentGameState = GameState.Playing;
            Game.audio.UnPause();
        }
    }

    private static void PlayLevel()
    {
        OnGameStart?.Invoke();
        UIManager.SetState("Game");
        if (Game.audio.isPlaying)
        {
            Game.audio.UnPause();
        }
        else
        {
            Game.audio.Play();
        }
    }

    //Goes to the main menu
    public async static void MainMenu()
    {
        UIManager.SetState("Main Menu");
        if (LevelManager.CurrentLevel != 0)
        {
            OnLevelEnd?.Invoke();
            await LevelManager.UnloadCurrentLevel();
        }
    }

    //Called when any "Main Menu" button is pressed
    public void MainMenuButton()
    {
        MainMenu();
    }

    private static async void LoadLevel()
    {
        if (LevelManager.CurrentLevel != 0)
        {
            OnLevelEnd?.Invoke();
            await LevelManager.UnloadCurrentLevel();
        }
        await LevelManager.LoadLevel(SelectedLevel);
        Game.audio.clip = Level.LevelMusic;
        UIManager.SetState("Ready");
        await Task.Run(() => Thread.Sleep(2000));
        UIManager.SetState("Game");
        CurrentGameState = GameState.Playing;
    }

    //Retries the current level. Called when the "Retry Level" button is pressed
    public void RetryLevel()
    {
        LoadLevel();
    }

    public async static void NextLevel()
    {
        SelectedLevel++;
        if (LevelManager.CurrentLevel != 0)
        {
            OnLevelEnd?.Invoke();
            await LevelManager.UnloadCurrentLevel();
        }
        CurrentGameState = GameState.Loading;
    }

    //Called when the "Next Level" button is pressed
    public void NextLevelButton()
    {
        NextLevel();
    }

    //Called when the "Play" button is pressed
    public void PlayGame()
    {
        SelectedLevel = 1;
        CurrentGameState = GameState.Loading;
    }

    public void SelectLevel()
    {
        UIManager.SetState("Pick Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
