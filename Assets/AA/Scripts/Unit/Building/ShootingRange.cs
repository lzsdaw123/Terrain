using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingRange : MonoBehaviour
{
    public static bool TargetWall; //啟動標靶
    public GameObject UI;
    public GameObject UIT;
    public GameObject Cam;
    public GameObject[] RangeWall;

    void Start()
    {
        TargetWall = false;
        //UI.SetActive(false);
        //UIT.SetActive(true);
        Cam.SetActive(false);
    }

    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)  //進入靶場
    {
        if(other.tag == "Player")
        {
            TargetWall = true;
            //UI.SetActive(true);
            //UIT.SetActive(false);
            Cam.SetActive(true);
        }        
    }
    public void OnTriggerExit(Collider other)  //離開靶場
    {
        if (other.tag == "Player")
        {
            TargetWall = false;
            UI.SetActive(false);
            //UIT.SetActive(true);
            Cam.SetActive(false);
        }
    }

}
