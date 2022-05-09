using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedButtle : MonoBehaviour
{
   // Renderer R1;
    public GameObject T;


    void Start()
    {
        //R1 = gameObject.GetComponent<Renderer>(); //把R1指定為物件的Renderer
        T = GameObject.Find("ObjectText");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        T.GetComponent<Text>().text = "按下";

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            print("按下紅色按鈕");
        }
    }
}
