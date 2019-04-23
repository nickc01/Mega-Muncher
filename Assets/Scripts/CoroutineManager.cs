using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager Singleton;
    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new static Coroutine StartCoroutine(IEnumerator routine)
    {
        return (Singleton as MonoBehaviour).StartCoroutine(routine);
    }

    public new static void StopCoroutine(IEnumerator routine)
    {
        (Singleton as MonoBehaviour).StopCoroutine(routine);
    }


}
