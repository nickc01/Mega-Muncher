using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentLevel { get; private set; } = 0; //The current level that is being played
    public static bool CoreLoaded { get; private set; } = false; //Set to true if the Core Scene is loaded

    //Unloads the current level
    public static async Task UnloadCurrentLevel()
    {
        bool Done = false;
        IEnumerator Unloader()
        {
            //Set the active scene to the core scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Core"));
            //Unload the level
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Level " + CurrentLevel));
            Done = true;
        }
        //If there is a level to unload
        if (CurrentLevel != 0)
        {
            //Run the unloader
            CoroutineManager.StartCoroutine(Unloader());
            //Wait until it is done
            await Task.Run(() => {
                while (Done == false) { }
            });
            CurrentLevel = 0;
        }
    }

    //Loads in a new level
    public static async Task LoadLevel(int Level)
    {
        //Unload the previous level if there is one
        await UnloadCurrentLevel();
        bool Done = false;
        IEnumerator Loader()
        {
            //Load the level scene
            yield return SceneManager.LoadSceneAsync("Level " + Level, LoadSceneMode.Additive);
            //Set it to be the active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level " + Level));
            Done = true;
        }
        //Start the loader
        CoroutineManager.StartCoroutine(Loader());
        //Wait until it is done
        await Task.Run(() => {
            while (Done == false) { }
        });
        //Update the current level
        CurrentLevel = Level;
    }
}
