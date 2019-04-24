using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Camera Main { get; private set; } //The Main Camera in the scene
    public static GameObject Target { get; set; } //The object the camera will be targeting
    [SerializeField] float Speed = 7f; //The speed the camera will move towards the target
    [SerializeField] Rect ScrollingArea; //An area, where, if the target leaves, the camera will move
    private static CameraManager MainManager; //The singleton for the camera
    void Start()
    {
        var camera = GetComponent<Camera>();
        if (camera == null)
        {
            Destroy(this);
            return;
        }
        //Set the singleton
        if (Main == null)
        {
            //Set the main camera
            Main = camera;
            MainManager = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //Sets the target and sets the camera's position to be at the target instantly
    public static void SetTargetForceful(GameObject target)
    {
        //Update the target
        Target = target;
        //Set the camera's position to the target
        MainManager.transform.position = new Vector3(target.transform.position.x,target.transform.position.y,MainManager.transform.position.z);
    }

    private void Update()
    {
        //If there is a target set
        if (Target != null)
        {
            //Get the screen space position of the target
            var screenSpace = Main.WorldToScreenPoint(Target.transform.position);
            //Get the screen space position of the camera. This should be the center of the screen
            var cameraScreen = Main.WorldToScreenPoint(Main.transform.position);
            //The X and Y target the camera will go towards
            float XTarget = 0f;
            float YTarget = 0f;
            
            //If the scrolling area intersects with itself on the x axis
            if (ScrollingArea.xMin > Main.pixelWidth - ScrollingArea.xMax)
            {
                //Ignore the scrolling area and move towards the target
                XTarget = screenSpace.x - cameraScreen.x;
            }
            else
            {
                //If the target's x position is outside of the scrolling area towards the left
                if (screenSpace.x < ScrollingArea.xMin)
                {
                    //Update the x target to move towards the player
                    XTarget = -(ScrollingArea.xMin - screenSpace.x);
                }
                //If the target's x position is outside of the scrolling area towards the right
                if (screenSpace.x > Main.pixelWidth - ScrollingArea.xMax)
                {
                    //Update the x target to move towards the player
                    XTarget = screenSpace.x - (Main.pixelWidth - ScrollingArea.xMax);
                }
            }
            //If the scrolling area intersects with itself on the y axis
            if (ScrollingArea.yMin > Main.pixelHeight - ScrollingArea.yMax)
            {
                //Ignore the scrolling area and move towards the target
                YTarget = screenSpace.y - cameraScreen.y;
            }
            else
            {
                //If the target's y position is outside of the scrolling area towards the bottom
                if (screenSpace.y < ScrollingArea.yMin)
                {
                    //Update the y target to move towards the player
                    YTarget = -(ScrollingArea.yMin - screenSpace.y);
                }
                //If the target's y position is outside of the scrolling area towards the top
                if (screenSpace.y > Main.pixelHeight - ScrollingArea.yMax)
                {
                    //Update the y target to move towards the player
                    YTarget = screenSpace.y - (Main.pixelHeight - ScrollingArea.yMax);
                }
            }
            //Debug lines to show where the scrolling area is
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, ScrollingArea.yMin)), Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, Main.pixelHeight - ScrollingArea.yMax)),Color.cyan);
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, Main.pixelHeight - ScrollingArea.yMax)), Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, Main.pixelHeight - ScrollingArea.yMax)), Color.cyan);
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, Main.pixelHeight - ScrollingArea.yMax)), Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, ScrollingArea.yMin)), Color.cyan);
            Debug.DrawLine(Main.ScreenToWorldPoint(new Vector3(Main.pixelWidth - ScrollingArea.xMax, ScrollingArea.yMin)), Main.ScreenToWorldPoint(new Vector3(ScrollingArea.xMin, ScrollingArea.yMin)), Color.cyan);
            //Move towards the player based on the X and Y Target information
            transform.position = Vector3.Lerp(transform.position,Main.ScreenToWorldPoint(cameraScreen + new Vector3(XTarget,YTarget)),Speed * Time.deltaTime);
        }
    }
}
