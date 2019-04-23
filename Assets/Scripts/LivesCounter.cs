using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesCounter : MonoBehaviour
{
    private static int lives = 0;
    public static int Lives
    {
        get => lives;
        set
        {
            lives = value;
            foreach (var counter in counters)
            {
                counter.UpdateText(value);
            }
        }
    }
    private static List<LivesCounter> counters = new List<LivesCounter>();

    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        counters.Add(this);
        UpdateText(Lives);
    }
    private void UpdateText(int livesAmount)
    {
        text.text = "Lives : " + livesAmount;
    }
}
