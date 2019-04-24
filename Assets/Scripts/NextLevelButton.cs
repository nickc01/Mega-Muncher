using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    new Image renderer; //The image component of the button
    Button button; //The button component of the button

    //When the win screen is enabled
    private void OnEnable()
    {
        //Get the components if they are not already set
        if (button == null)
        {
            button = GetComponent<Button>();
            renderer = GetComponent<Image>();
        }
        //Enable or disable the button depending on which level the game is on to make sure that you do not go to a level that does not exist
        button.enabled = GameManager.SelectedLevel < GameManager.Levels;
        renderer.enabled = GameManager.SelectedLevel < GameManager.Levels;
        transform.GetChild(0).gameObject.SetActive(GameManager.SelectedLevel < GameManager.Levels);
    }
}
