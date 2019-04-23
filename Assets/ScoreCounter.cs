using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private static int score = 0;
    public static int Score
    {
        get => score;
        set
        {
            score = value;
            foreach (var counter in counters)
            {
                counter.UpdateText(value);
            }
        }
    }
    private static List<ScoreCounter> counters = new List<ScoreCounter>();

    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        counters.Add(this);
        UpdateText(Score);
    }
    private void UpdateText(int scoreAmount)
    {
        text.text = "Score : " + scoreAmount;
    }
}
