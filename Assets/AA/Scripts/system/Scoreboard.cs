using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public static int Score;
    public Text Text;
    public Text SettlementText;
    public Text PlayDeadText;
    public Text MonsterLVText;
    public Text DiffLevelText;
    static bool SettlementTF = false;
    static int DeadScore;
    int MonsterLevel;  //怪物等級
    int Level;  //難度等級
    public string[] DiffT = new string[] { "簡單", "普通", "困難" };
    void Start()
    {
        Score= DeadScore = 0;
    }

    void Update()
    {
        MonsterLevel = Level_1.MonsterLevel;
        Text.text = "怪物擊殺數 : " + Score;
        PlayDeadText.text = "玩家死亡數 : " + DeadScore;
        MonsterLVText.text = "怪物等級 : " + MonsterLevel;
        if (MissionTarget_Life.Dead)
        {
            Level = Settings.Level;
            int Total = (Score * 20) - (DeadScore * 100) + (Level * 200);  //擊殺數*20 -死亡數*100 +難度*200  
            DiffLevelText.text = "遊戲難度 : " + DiffT[Level];
            if (SettlementTF)
            {
                SettlementTF = false;              
                SettlementText.text = "遊戲分數 : " + Total;
            }
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
        DeadScore += Dead;
    }
}
