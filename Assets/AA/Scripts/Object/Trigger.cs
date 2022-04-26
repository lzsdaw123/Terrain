using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int Level;
    public int Type;
    public GameObject[] Objects;
    //public Level_1 level_1;

    void Start()
    {
    }

    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)  //Ä²µoÃö¥d
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                GameObject.Find("Level_1").GetComponent<Level_1>().MissionTrigger(Level, Objects[Type]);
                gameObject.SetActive(false);
            }
        }
    }
}
