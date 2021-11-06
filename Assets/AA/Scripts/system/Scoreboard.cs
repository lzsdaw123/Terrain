using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    static int Score;
    public Text Text;
    public Text SettlementText;
    public Text PlayDeadText;
    static bool SettlementTF = false;
    static int DeadScore;


    void Start()
    {
        Score= DeadScore = 0;
    }

    void Update()
    {
        Text.text = "怪物擊殺數 : " + Score;
        PlayDeadText.text = "玩家死亡數 : " + DeadScore;
        if (SettlementTF)
        {
            SettlementTF = false;
            int Total = (Score * 20) - (DeadScore * 100);
            SettlementText.text = "遊戲分數 : " + Total;
        }
    }
    public static void AddScore(bool St)
    {
        if (St)
        {
            St = false;
            Score+=1;
        }
    }
    public static void Settlement()
    {
        SettlementTF = true;
    }
    public static void ReScore(int Dead)
    {
        DeadScore = Dead;
    }
}
