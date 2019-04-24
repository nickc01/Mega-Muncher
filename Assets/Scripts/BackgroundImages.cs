using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImages : MonoBehaviour
{
    private static BackgroundImages Singleton;
    public GameObject Prefab; //The background prefab that will be cloned around the map
    public int Amount = 100; //The amount of clones to make
    public Rect Boundaries; //The area that the clones are restricted to
    public float MinDepth = 0f; //The minimum z-depth away from the camera
    public float MaxDepth = 100f; //The maximum z-depth away from the camera
    public Color FrontColor; //The color the clones nearest to the camera will have


    private static Camera cam; //The reference to the camera. Used to get the skybox color

    static List<(SpriteRenderer,float)> clones = new List<(SpriteRenderer, float)>(); //The list of clones in the map

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
        //Set the camera reference
        cam = GetComponent<Camera>();
        //Spawn a set amount of clones and give it a random position based on the Boundaries, MinDepth, and MaxDepth
        for (int i = 0; i < Amount; i++)
        {
            var Z = Random.Range(MinDepth, MaxDepth);
            var newObject = GameObject.Instantiate(Prefab, new Vector3(Random.Range(Boundaries.xMin, Boundaries.xMax), Random.Range(Boundaries.yMin, Boundaries.yMax), Z), Quaternion.identity).GetComponent<SpriteRenderer>();
            //Add it to the list of clones
            clones.Add((newObject,Z));
            //Give each clone a color depending on how far away it is
            newObject.color = Color.Lerp(FrontColor,cam.backgroundColor,Z / MaxDepth);
        }
    }

    //Sets the color of all the clones
    public static void SetColor(Color color, Color Secondary)
    {
        //Set the skybox to the secondary color
        cam.backgroundColor = Secondary;
        foreach (var obj in clones)
        {
            //Set the color of all the clones
            obj.Item1.color = Color.Lerp(color, cam.backgroundColor, obj.Item2 / Singleton.MaxDepth);
        }
    }
}
