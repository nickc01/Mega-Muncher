using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//A list of states for the game
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
    [SerializeField] int totalLevels = 1; //The total amount of levels in the game
    public static int Levels => Game.totalLevels; //A static interface for accessing the total levels
    private static GameState gameStateInternal = GameState.Menu; //The current state of the game
    public static GameState CurrentGameState //The public interface for accessing the game state
    {
        get => gameStateInternal;
        set
        {
            if (gameStateInternal == value)
            {
                return;
            }
            gameStateInternal = value;
            //Update the state based on the value
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
                    BeginPlaying();
                    break;
                case GameState.Loading: //Starting the game with the selected level
                    LoadLevel();
                    break;
            }
        }
    }
    public static int SelectedLevel = 0; //The selected level that will be loaded
    public static GameManager Game { get; private set; } //The singleton for the game manager
    [Header("Prefabs")]
    public GameObject PelletPrefab; //The prefab for the pellets
    public GameObject MuncherPrefab; //The prefab for the muncher
    public GameObject GhostPrefab; //The prefab for the ghosts
    public GameObject TeleporterPrefab; //The prefab for the teleporters
    public GameObject ExplosionPrefab; //The prefab for the explosions
    [Header("Sounds")]
    public AudioClip PelletSound; //The sound played when the muncher eats a pellet
    public AudioClip PowerUpSound; //The sound played when the muncher collects a power up
    public AudioClip EatGhostSound; //The sound played when the muncher eats a vulnerable ghost

    private new AudioSource audio; //The audio component used to play the level music


    //Called when the player wins the game
    public static event Action OnWin;
    //Called when the player looses a life
    public static event Action OnLose;
    //Called on either winning the game or loosing a life
    public static event Action OnGamePause;
    //Called to set everything in motion when the game starts
    public static event Action OnGameStart;
    //Called to clean up the level before it is unloaded
    public static event Action OnLevelUnload;
    //Called to reset the level's objects to their starting state
    public static event Action OnLevelReset;
    // Start is called before the first frame update
    void Start()
    {
        //Set the singleton
        if (Game == null)
        {
            Game = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //Get the audio component
        audio = GetComponent<AudioSource>();
    }

    //Called when the player collects all the pellets and wins the game
    private async static void Win()
    {
        //Pause all the objects in the game
        OnGamePause?.Invoke();
        //Trigger the win event
        OnWin?.Invoke();
        //Pause the game music
        Game.audio.Pause();
        //Wait for a second
        await Task.Run(() => Thread.Sleep(1000));
        //Show the win screen
        if (Application.isPlaying)
        {
            UIManager.SetState("Win Screen");
        }
    }

    //Called when the muncher gets hit by a ghost
    private async static void Lose()
    {
        //Pause all the objects in the game
        OnGamePause?.Invoke();
        //Trigger the lose event
        OnLose?.Invoke();
        //Pause the game music
        Game.audio.Pause();
        //Wait for a second
        await Task.Run(() => Thread.Sleep(1000));
        if (!Application.isPlaying)
        {
            return;
        }
        //Reduce the lives counter
        Muncher.Lives--;
        //If there are no more lives left
        if (Muncher.Lives == 0)
        {
            //Show the lose screen
            UIManager.SetState("Lose Screen");
            //Stop the game music
            Game.audio.Stop();
        }
        //If there are still lives left
        else
        {
            //Reset the objects in the level
            OnLevelReset?.Invoke();
            //Show the ready screen
            UIManager.SetState("Ready");
            //Wait for two seconds
            await Task.Run(() => Thread.Sleep(2000));
            if (!Application.isPlaying)
            {
                return;
            }
            //Show the main game HUD
            UIManager.SetState("Game");
            //Set everything in the level into motion
            CurrentGameState = GameState.Playing;
            //Unpause the game music
            Game.audio.UnPause();
        }
    }

    //Sets everything into motion
    private static void BeginPlaying()
    {
        //Start all the objects to get them to move
        OnGameStart?.Invoke();
        //Show the main game HUD
        UIManager.SetState("Game");
        //If the music is still playing
        if (Game.audio.isPlaying)
        {
            //Unpause the music
            Game.audio.UnPause();
        }
        else
        {
            //Play the music
            Game.audio.Play();
        }
        //Check for any pellets left in the game
        Pellet.CheckForPellets();
    }

    //Goes to the main menu
    public async static void MainMenu()
    {
        //Show the main menu screen
        UIManager.SetState("Main Menu");
        //If there is a level still loaded
        if (LevelManager.CurrentLevel != 0)
        {
            //Tell all objects in the scene to close down and cleaup up for th next level
            OnLevelUnload?.Invoke();
            //Unload the level
            await LevelManager.UnloadCurrentLevel();
        }
    }

    //Called when any "Main Menu" button is pressed
    public void MainMenuButton()
    {
        //Go to the main menu
        MainMenu();
    }

    private static async void LoadLevel()
    {
        //If there is a level still loaded
        if (LevelManager.CurrentLevel != 0)
        {
            //Tell all objects in the scene to close down and cleaup up for th next level
            OnLevelUnload?.Invoke();
            //Unload the level
            await LevelManager.UnloadCurrentLevel();
        }
        //Load the selected level
        await LevelManager.LoadLevel(SelectedLevel);
        //Set the game music to the music for the level
        Game.audio.clip = Level.LevelMusic;
        //Show the ready screen
        UIManager.SetState("Ready");
        //Wait for two seconds
        await Task.Run(() => Thread.Sleep(2000));
        if (!Application.isPlaying)
        {
            return;
        }
        //Show the main game HUD
        UIManager.SetState("Game");
        //Set everything into motion
        CurrentGameState = GameState.Playing;
    }

    //Retries the current level. Called when the "Retry Level" button is pressed
    public void RetryLevel()
    {
        //Reload the current level
        LoadLevel();
    }

    //Used to go to the next level
    public async static void NextLevel()
    {
        //Go to the next level
        SelectedLevel++;
        //If there is a level still loaded
        if (LevelManager.CurrentLevel != 0)
        {
            //Tell all objects in the scene to close down and cleaup up for th next level
            OnLevelUnload?.Invoke();
            //Unload the level
            await LevelManager.UnloadCurrentLevel();
        }
        //Load the next level
        CurrentGameState = GameState.Loading;
    }

    //Called when the "Next Level" button is pressed
    public void NextLevelButton()
    {
        //Go to the next level
        NextLevel();
    }

    //Called when the "Play" button is pressed
    public void PlayGame()
    {
        //Go to the first level
        SelectedLevel = 1;
        //Load the level
        CurrentGameState = GameState.Loading;
    }

    //Called when the "Select Level" button is pressed
    public void SelectLevel()
    {
        //Show the level select screen
        UIManager.SetState("Pick Level");
    }

    //Called when the "Quit" button is pressed
    public void QuitGame()
    {
        //Quit the application
        Application.Quit();
    }
}
