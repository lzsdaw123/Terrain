using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QH_interactive : MonoBehaviour
{
    Ray ray; //射線
    float raylength = 2f; //射線最大長度
    RaycastHit hit; //被射線打到的物件

    public GameObject T;


    void Start()
    {
        T = GameObject.Find("ObjectText");
    }

    void Update()
    {
        //ray = Camera.current.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //由攝影機射到是畫面正中央的射線
        ray = new Ray(transform.position, transform.forward);
        T.GetComponent<Text>().text = "";

        if (Physics.Raycast(ray, out hit, raylength))
        // (射線,out 被射線打到的物件,射線長度)，out hit 意思是：把"被射線打到的物件"帶給hit
        {
            hit.transform.SendMessage("HitByRaycast", gameObject, SendMessageOptions.DontRequireReceiver);
            //向被射線打到的物件呼叫名為"HitByRaycast"的方法，不需要傳回覆

            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
            //當射線打到物件時會在Scene視窗畫出黃線，方便查閱

            //print(hit.transform.name);
            //在Console視窗印出被射線打到的物件名稱，方便查閱                       
        }
        else
        {
            T.GetComponent<Text>().text = "";
        }
    }
}
