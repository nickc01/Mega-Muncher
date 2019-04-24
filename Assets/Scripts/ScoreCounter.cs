using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private static int score = 0; //The score counter 
    public static int Score //The public interface for accessing the score
    {
        get => score;
        set
        {
            score = value;
            foreach (var counter in counters)
            {
                //Update the score text in the HUD
                counter.UpdateText(value);
            }
        }
    }
    private static List<ScoreCounter> counters = new List<ScoreCounter>(); //All the score text counters in the game

    private TextMeshProUGUI text; //The text component of the counter
    // Start is called before the first frame update
    void Start()
    {
        //Get the text component
        text = GetComponent<TextMeshProUGUI>();
        //Add this counter to the list of counters
        counters.Add(this);
        //Update the score text
        UpdateText(Score);
    }
    //Updates the score text
    private void UpdateText(int scoreAmount)
    {
        text.text = "Score : " + scoreAmount;
    }
}
