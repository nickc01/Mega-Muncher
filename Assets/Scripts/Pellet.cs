using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour, IEatable
{
    public static List<Pellet> Pellets = new List<Pellet>(); //All the pellets that are currently in the level
    protected virtual bool DestroyOnEat => true;

    public virtual void Start()
    {
        Pellets.Add(this);
    }

    [RuntimeInitializeOnLoadMethod]
    public static void LoadInitialization()
    {
        //Adds an event to clear the pellet list when the level unloads
        GameManager.OnLevelUnload += () => Pellets.Clear();
    }

    //When the pellet is eaten
    public virtual void OnEat(Muncher muncher)
    {
        //Play the pellet sound
        AudioSource.PlayClipAtPoint(GameManager.Game.PelletSound, transform.position);
        if (Pellets.Contains(this))
        {
            //Increase the score
            ScoreCounter.Score += 1;
            //Remove this pellet from the list of pellets
            Pellets.Remove(this);
            if (DestroyOnEat)
            {
                //Destroy this pellet from the level
                Destroy(gameObject);
            }
            //If there are no more pellets in the level
            if (Pellets.Count == 0)
            {
                //Win the game
                GameManager.CurrentGameState = GameState.Win;
            }
        }
    }
}
