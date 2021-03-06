using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public int Type;
    public GameObject play;
    public float distance;

    void Start()
    {
        play = Save_Across_Scene.Play;
    }

    // Update is called once per frame
    void Update()
    {
        //distance = (play.transform.position - transform.position).magnitude / 3.5f;



    }
    public void ExitGame()
    {
        Settings.pause();
        Settings.ExitGame();
    }
    public void OnTriggerEnter(Collider other)  //觸發關卡
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                switch (Type)
                {
                    case 0:
                        DontDestroyOnLoad(other.gameObject);  //切換場景時保留
                        other.gameObject.SetActive(false);
                        Level_1.BG_Level = 2;
                        Settings.GameLevel = 2;
                        AudioManager.AudioStop = true;
                        Settings.Enter_Scene2();
                        PlayerView.missionLevel = 4;
                        PlayerView.missionStage = 0;
                        PlayerView.UI_Stop = true;
                        HeroLife.HpLv = 2;
                        Save_Across_Scene.heroLife.closeDamageEffects(); //關閉受傷特效
                        Save_Across_Scene.Shooting.closeFireEffects(); //關閉攻擊特效
                        Level_1.StartTime = -1;
                        Level_1.MonsterDead = false;
                        ObjectPool.color = true;
                        break;
                    case 1:
                        ExitGame();
                        break;
                }
            }
        }
    }
}
