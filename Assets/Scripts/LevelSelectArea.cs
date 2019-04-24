using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectArea : MonoBehaviour
{
    [SerializeField] GameObject LevelButtonPrefab; //The prefab for the level buttons
    async void Start()
    {
        //Wait a small amount of time
        await Task.Run(() => Thread.Sleep(100));
        //Loop over all the levels
        for (int i = 0; i < GameManager.Levels; i++)
        {
            var level = i + 1;
            //Create a new level button
            var newButton = GameObject.Instantiate(LevelButtonPrefab, transform);
            //When the button is clicked, start the level
            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameManager.SelectedLevel = level;
                GameManager.CurrentGameState = GameState.Loading;
            });
            //Update the button's text to the corresponding level
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = "Level " + level;
        }
    }
}
