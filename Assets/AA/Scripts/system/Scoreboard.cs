using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    static int Score;
    public Text Text;

    void Start()
    {
        Score = 0;
    }

    void Update()
    {
        Text.text = "怪物擊殺數 : " + Score;
    }
    public static void AddScore(bool St)
    {
        if (St)
        {
            St = false;
            Score++;
        }
    }
}
