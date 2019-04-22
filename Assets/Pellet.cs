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

    public virtual void OnEat(Muncher muncher)
    {
        if (Pellets.Contains(this))
        {
            Pellets.Remove(this);
            if (DestroyOnEat)
            {
                Destroy(gameObject);
            }
            if (Pellets.Count == 0)
            {
                Debug.Log("WIN");
            }
        }
    }
}
