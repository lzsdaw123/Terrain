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
    public void OnTriggerEnter(Collider other)  //Ĳ�o���d
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                switch (Type)
                {
                    case 0:
                        DontDestroyOnLoad(other.gameObject);  //���������ɫO�d
                        other.gameObject.SetActive(false);
                        Settings.Enter_Scene2();
                        PlayerView.missionLevel = 4;
                        PlayerView.missionStage = 0;
                        HeroLife.HpLv = 2;
                        break;
                    case 1:
                        ExitGame();
                        break;
                }
            }
        }
    }
}
