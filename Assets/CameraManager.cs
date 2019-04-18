using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Camera Main { get; private set; } //The Main Camera in the scene
    void Start()
    {
        var camera = GetComponent<Camera>();
        if (camera == null)
        {
            Destroy(this);
            return;
        }
        if (Main == null)
        {
            Main = camera;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
