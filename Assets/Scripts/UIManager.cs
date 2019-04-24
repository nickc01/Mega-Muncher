using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static string CurrentState = "Main Menu"; //The current UI state
    private static Dictionary<string, GameObject> UIStates = new Dictionary<string, GameObject>(); //All the possible UI states
    private static UIManager Singleton; //The singleton for the UI Manager
    void Start()
    {
        //Set the singleton
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //Get all the possible UI states
        for (int c = 0; c < transform.childCount; c++)
        {
            var child = transform.GetChild(c).gameObject;
            child.SetActive(false);
            UIStates.Add(child.name, child);
        }
        //Update the current state
        SetState(CurrentState);
    }

    //Sets the new state for the UI
    public static void SetState(string NewState)
    {
        //Disable the previous state
        UIStates[CurrentState].SetActive(false);
        CurrentState = NewState;
        //If the new state exists
        if (UIStates.ContainsKey(NewState))
        {
            //Enable it
            UIStates[NewState].SetActive(true);
        }
        else
        {
            //Throw an exception if it does not exist
            throw new Exception(NewState + " is an invalid state");
        }
    }
}
