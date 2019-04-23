using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour, IEatable
{
    public static List<Pellet> Pellets = new List<Pellet>();
    protected virtual bool DestroyOnEat => true;

    public virtual void Start()
    {
        Pellets.Add(this);
    }

    [RuntimeInitializeOnLoadMethod]
    public static void LoadInitialization()
    {
        GameManager.OnLevelEnd += () => Pellets.Clear();
    }

    public virtual void OnEat(Muncher muncher)
    {
        AudioSource.PlayClipAtPoint(GameManager.Game.PelletSound, transform.position);
        if (Pellets.Contains(this))
        {
            ScoreCounter.Score += 1;
            Pellets.Remove(this);
            if (DestroyOnEat)
            {
                Destroy(gameObject);
            }
            if (Pellets.Count == 0)
            {
                GameManager.CurrentGameState = GameState.Win;
            }
        }
    }
}
