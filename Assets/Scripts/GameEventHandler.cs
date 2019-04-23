using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameEventHandler : MonoBehaviour
{
    protected virtual bool AutoRemoveEvents => true;
    protected void StartEvents()
    {
        GameManager.OnWin += OnWin;
        GameManager.OnLose += OnLose;
        GameManager.OnGamePause += OnGamePause;
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnLevelEnd += OnLevelEnd;
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
            GameManager.OnWin -= OnWin;
            GameManager.OnLose -= OnLose;
            GameManager.OnGamePause -= OnGamePause;
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnLevelEnd -= OnLevelEnd;
            GameManager.OnLevelReset -= OnLevelReset;
        }
    }
    protected virtual void OnLevelReset() { }
}
