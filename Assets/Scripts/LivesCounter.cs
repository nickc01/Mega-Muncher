using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesCounter : MonoBehaviour
{
    private static int lives = 0; //The lives counter
    public static int Lives //The public interface for accessing the lives
    {
        get => lives;
        set
        {
            lives = value;
            foreach (var counter in counters)
            {
                //Update the lives text in the HUD
                counter.UpdateText(value);
            }
        }
    }
    private static List<LivesCounter> counters = new List<LivesCounter>(); //The counters that are active in the scene

    private TextMeshProUGUI text; //The text that will display the lives
    // Start is called before the first frame update
    void Start()
    {
        //Get the text component
        text = GetComponent<TextMeshProUGUI>();
        //Add this object to the list of counters
        counters.Add(this);
        //Update the lives counter
        UpdateText(Lives);
    }
    //Updates the text to display the current amount of lives
    private void UpdateText(int livesAmount)
    {
        text.text = "Lives : " + livesAmount;
    }
}
