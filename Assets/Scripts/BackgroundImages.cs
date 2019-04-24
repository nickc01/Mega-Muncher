using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImages : MonoBehaviour
{
    public GameObject Prefab;
    public int Amount = 100;
    public Rect Boundaries;
    public float MinDepth = 0f;
    public float MaxDepth = 100f;
    public Color FrontColor;


    private static Camera cam;
    private static float MaxDepthStatic;

    static List<(SpriteRenderer,float)> objects = new List<(SpriteRenderer, float)>();

    void Start()
    {
        cam = GetComponent<Camera>();
        MaxDepthStatic = MaxDepth;
        for (int i = 0; i < Amount; i++)
        {
            var Z = Random.Range(MinDepth, MaxDepth);
            var newObject = GameObject.Instantiate(Prefab, new Vector3(Random.Range(Boundaries.xMin, Boundaries.xMax), Random.Range(Boundaries.yMin, Boundaries.yMax), Z), Quaternion.identity).GetComponent<SpriteRenderer>();
            objects.Add((newObject,Z));
            newObject.color = Color.Lerp(FrontColor,cam.backgroundColor,Z / MaxDepth);
        }
    }

    public static void SetColor(Color color, Color Secondary)
    {
        cam.backgroundColor = Secondary;
        foreach (var obj in objects)
        {
            obj.Item1.color = Color.Lerp(color, cam.backgroundColor, obj.Item2 / MaxDepthStatic);
        }
    }
}
