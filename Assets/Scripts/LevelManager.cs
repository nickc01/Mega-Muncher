using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentLevel { get; private set; } = 0; //The current level in currently being played
    public static bool CoreLoaded { get; private set; } = false; //Set to true if the Core Scene is loaded

    [RuntimeInitializeOnLoadMethod]
    static void StartLevelLoad()
    {
        var activeScene = SceneManager.GetActiveScene();
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
