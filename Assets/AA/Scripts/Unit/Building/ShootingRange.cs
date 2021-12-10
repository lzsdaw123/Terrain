using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingRange : MonoBehaviour
{
    public static bool TargetWall;
    public GameObject UI;
    public GameObject UIT;
    public GameObject Cam;

    void Start()
    {
        TargetWall = false;
        UI.SetActive(false);
        UIT.SetActive(true);
        Cam.SetActive(false);
    }

    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            TargetWall = true;
            UI.SetActive(true);
            UIT.SetActive(false);
            Cam.SetActive(true);
        }        
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            TargetWall = false;
            UI.SetActive(false);
            UIT.SetActive(true);
            Cam.SetActive(false);
        }
    }

}
