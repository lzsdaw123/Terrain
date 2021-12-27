using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QH_interactive : MonoBehaviour
{
    Ray ray; //射線
    float raylength = 3f; //射線最大長度
    RaycastHit hit; //被射線打到的物件
    RaycastHit oldhit; //被射線打到的物件

    public LayerMask layerMask;
    public GameObject ObjectText;
    public static GameObject Take;
    public static bool tt;
    public bool ret;
    void Start()
    {
        ObjectText = GameObject.Find("ObjectText");
        Take = GameObject.Find("Take");
        Take.SetActive(false);
    }

    void Update()
    {
        //由攝影機射到是畫面正中央的射線
        ray = gameObject.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        ObjectText.GetComponent<Text>().text = "";

        int maskActor = 1 << LayerMask.NameToLayer("Actor");
        if(Physics.Raycast(ray, out hit, raylength, maskActor))  //NPC互動
        {
            if (hit.collider.tag == "NPC")
            {
                hit.transform.SendMessage("HitByRaycast", gameObject, SendMessageOptions.DontRequireReceiver);
                if (hit.collider == null)
                {
                    return;
                }
                else if (hit.collider == oldhit.collider)
                {
                    return;
                }
                else if (hit.collider != oldhit.collider)
                {
                    Take.SetActive(false);
                }
                oldhit = hit;
            }
        }

        // (射線,out 被射線打到的物件,射線長度)，out hit 意思是：把"被射線打到的物件"帶給hit
        if (Physics.Raycast(ray, out hit, raylength, layerMask))
        {           
            hit.transform.SendMessage("HitByRaycast", gameObject, SendMessageOptions.DontRequireReceiver);
            //向被射線打到的物件呼叫名為"HitByRaycast"的方法，不需要傳回覆
  

            if (hit.collider == null)
            {
                return;
            }
            else if (hit.collider == oldhit.collider)
            {
                return;
            }
            else if (hit.collider != oldhit.collider)
            {
                Take.SetActive(false);
            }
            oldhit = hit;



            //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f);
            //當射線打到物件時會在Scene視窗畫出黃線，方便查閱
            //print(hit.collider.name);
            //在Console視窗印出被射線打到的物件名稱，方便查閱
        }
        else
        {
            ObjectText.GetComponent<Text>().text = "";
            Take.SetActive(false);
        }
    }
    public static void thing()
    {
        Take.SetActive(true);
    }
}
