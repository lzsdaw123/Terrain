using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Life : MonoBehaviour
{
    public float UI_Time;
    public float UI_LiftTime=3;
    void Start()
    {
        UI_Time = 0;
    }

    void Update()
    {
        UI_Time += Time.deltaTime;
        if (UI_Time >= UI_LiftTime)
        {
            UI_Time = 0;
            gameObject.SetActive(false);
        }
    }
}
