using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager Singleton; //The singleton for the coroutine manager
    // Start is called before the first frame update
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
    }

    //Allows starting a coroutine from a static context
    public new static Coroutine StartCoroutine(IEnumerator routine)
    {
        return (Singleton as MonoBehaviour).StartCoroutine(routine);
    }

    //Stops a coroutine from a static context
    public new static void StopCoroutine(IEnumerator routine)
    {
        (Singleton as MonoBehaviour).StopCoroutine(routine);
    }


}
