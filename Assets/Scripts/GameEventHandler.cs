using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//An easier way to interact with the Game Manager's events
public abstract class GameEventHandler : MonoBehaviour
{
    protected virtual bool AutoRemoveEvents => true; //Determines if the events are automatically removed when the level ends
    //Adds the events to the GameManager
    protected void StartEvents()
    {
        GameManager.OnWin += OnWin;
        GameManager.OnLose += OnLose;
        GameManager.OnGamePause += OnGamePause;
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnLevelUnload += OnLevelEnd;
        GameManager.OnLevelReset += OnLevelReset;
    }
    protected virtual void OnWin() { }
    protected virtual void OnLose() { }
    protected virtual void OnGamePause() { }
    protected virtual void OnGameStart() { }
    protected virtual void OnLevelEnd()
    {
        if (AutoRemoveEvents)
        {
            //Remove the events when the level ends
            GameManager.OnWin -= OnWin;
            GameManager.OnLose -= OnLose;
            GameManager.OnGamePause -= OnGamePause;
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnLevelUnload -= OnLevelEnd;
            GameManager.OnLevelReset -= OnLevelReset;
        }
    }
    protected virtual void OnLevelReset() { }
}
