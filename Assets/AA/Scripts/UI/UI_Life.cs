using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Life : MonoBehaviour
{
    public int Type; 
    public float UI_Time;
    public float UI_LiftTime=3;
    void Start()
    {
        UI_Time = 0;
    }

    void Update()
    {
        switch (Type)
        {
            case 0:
                UI_Time += Time.deltaTime;
                if (UI_Time >= UI_LiftTime)
                {
                    UI_Time = 0;
                    gameObject.SetActive(false);
                }
                break;
            case 1:
                DontDestroyOnLoad(gameObject);  //切換場景時保留
                int SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
                if (SceneNub == 1)
                {
                    Destroy(gameObject);
                }
                break;
        }     
    }
}
