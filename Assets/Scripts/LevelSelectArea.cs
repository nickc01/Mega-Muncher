using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectArea : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject LevelButtonPrefab;
    async void Start()
    {
        await Task.Run(() => Thread.Sleep(100));
        for (int i = 0; i < GameManager.Levels; i++)
        {
            var level = i + 1;
            var newButton = GameObject.Instantiate(LevelButtonPrefab, transform);
            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameManager.SelectedLevel = level;
                GameManager.CurrentGameState = GameState.Loading;
            });
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = "Level " + level;
        }
    }
}
