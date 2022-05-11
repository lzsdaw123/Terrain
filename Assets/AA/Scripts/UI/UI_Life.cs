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
                DontDestroyOnLoad(gameObject);  //���������ɫO�d
                int SceneNub = SceneManager.GetActiveScene().buildIndex; //���o��e�����s��
                if (SceneNub == 1)
                {
                    Destroy(gameObject);
                }
                break;
        }     
    }
}
