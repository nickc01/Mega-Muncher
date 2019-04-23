using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentLevel { get; private set; } = 0; //The current level in currently being played
    public static bool CoreLoaded { get; private set; } = false; //Set to true if the Core Scene is loaded

    [RuntimeInitializeOnLoadMethod]
    static void StartLevelLoad()
    {
        //var activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var activeScene = SceneManager.GetSceneAt(i);
            if (activeScene.name.Contains("Level"))
            {
                CurrentLevel = int.Parse(Regex.Match(activeScene.name, @"Level\s*(\d+)").Groups[1].Value);
                if (SceneManager.GetSceneByName("Core").isLoaded == false)
                {
                    SceneManager.LoadScene("Core", LoadSceneMode.Additive);
                    SceneManager.sceneLoaded += (scene, loadMethod) =>
                    {
                        if (scene.name == "Core")
                        {
                            CoreLoaded = true;
                        }
                    };
                }
                else
                {
                    CoreLoaded = true;
                }
            }
        }
    }

    public static async Task UnloadCurrentLevel()
    {
        bool Done = false;
        IEnumerator Unloader()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Core"));
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Level " + CurrentLevel));
            Done = true;
        }
        if (CurrentLevel != 0)
        {
            CoroutineManager.StartCoroutine(Unloader());
            await Task.Run(() => {
                while (Done == false) { }
            });
            CurrentLevel = 0;
        }
    }

    public static async Task LoadLevel(int Level)
    {
        await UnloadCurrentLevel();
        bool Done = false;
        IEnumerator Loader()
        {
            yield return SceneManager.LoadSceneAsync("Level " + Level, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level " + Level));
            Done = true;
        }
        CoroutineManager.StartCoroutine(Loader());
        await Task.Run(() => {
            while (Done == false) { }
        });
        CurrentLevel = Level;
    }
}
