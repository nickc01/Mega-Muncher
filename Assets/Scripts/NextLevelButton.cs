using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    new Image renderer;
    Button button;

    private void OnEnable()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
            renderer = GetComponent<Image>();
        }
        button.enabled = GameManager.SelectedLevel < GameManager.Levels;
        renderer.enabled = GameManager.SelectedLevel < GameManager.Levels;
        transform.GetChild(0).gameObject.SetActive(GameManager.SelectedLevel < GameManager.Levels);
    }
}
