using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static string CurrentState = "Main Menu";
    private static Dictionary<string, GameObject> UIStates = new Dictionary<string, GameObject>();
    private static UIManager Singleton;
    void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (LevelManager.CurrentLevel != 0)
        {
            CurrentState = "Game";
        }
        for (int c = 0; c < transform.childCount; c++)
        {
            var child = transform.GetChild(c).gameObject;
            child.SetActive(false);
            UIStates.Add(child.name, child);
        }
        SetState(CurrentState);
    }

    public static void SetState(string NewState)
    {
        UIStates[CurrentState].SetActive(false);
        CurrentState = NewState;
        if (UIStates.ContainsKey(NewState))
        {
            UIStates[NewState].SetActive(true);
        }
        else
        {
            throw new Exception(NewState + " is an invalid state");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
