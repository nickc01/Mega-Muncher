using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Camera Main { get; private set; } //The Main Camera in the scene
    public static GameObject Target { get; set; }
    [SerializeField] float Speed = 7f;
    [SerializeField] Rect ScrollingArea;
    private static CameraManager MainManager;
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
            MainManager = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (Target != null)
        {
            var screenSpace = Main.WorldToScreenPoint(Target.transform.position);
            var cameraScreen = Main.WorldToScreenPoint(Main.transform.position);
            float XTarget = 0f;
            float YTarget = 0f;
            if (ScrollingArea.xMin > Main.pixelWidth - ScrollingArea.xMax)
            {
                XTarget = screenSpace.x - cameraScreen.x;
            }
            else
            {
                if (screenSpace.x < ScrollingArea.xMin)
                {
                    XTarget = -(ScrollingArea.xMin - screenSpace.x);
                }
                if (screenSpace.x > Main.pixelWidth - ScrollingArea.xMax)
                {
                    XTarget = screenSpace.x - (Main.pixelWidth - ScrollingArea.xMax);
                }
            }
            if (ScrollingArea.yMin > Main.pixelHeight - ScrollingArea.yMax)
            {
                YTarget = screenSpace.y - cameraScreen.y;
            }
            else
            {
                if (screenSpace.y < ScrollingArea.yMin)
                {
                    YTarget = -(ScrollingArea.yMin - screenSpace.y);
                }
                if (screenSpace.y > Main.pixelHeight - ScrollingArea.yMax)
                {
                    YTarget = screenSpace.y - (Main.pixelHeight - ScrollingArea.yMax);
                }
            }
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, ScrollingArea.yMin)), Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, Main.pixelHeight - ScrollingArea.yMax)));
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, Main.pixelHeight - ScrollingArea.yMax)), Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, Main.pixelHeight - ScrollingArea.yMax)));
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, Main.pixelHeight - ScrollingArea.yMax)), Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, ScrollingArea.yMin)));
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, ScrollingArea.yMin)), Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, ScrollingArea.yMin)));
            transform.position = Vector3.Lerp(transform.position,Main.ScreenToWorldPoint(cameraScreen + new Vector3(XTarget,YTarget)),Speed * Time.deltaTime);
        }
    }
}
