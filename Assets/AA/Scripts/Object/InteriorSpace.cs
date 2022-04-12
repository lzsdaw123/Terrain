using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSpace : MonoBehaviour
{
    public static bool Interior;  //室內
    public static bool Airtight;  //密閉
    public static int Deep;  //深處
    [SerializeField] bool 室內;
    [SerializeField] bool 密閉;
    [SerializeField] int 深處;
    public GameObject Door;
    public ElectricDoor _ElectricDoor;
    [SerializeField] bool CloseDoor;
    public float distance;
    public GameObject Player;
    public static float Pitch;
    [SerializeField] float SF_Pitch;

    void Start()
    {
        Interior = false;
        Pitch = 1;
    }

    void Update()
    {
        室內 = Interior;
        密閉 = Airtight;
        深處 = Deep;
        SF_Pitch = Pitch;
        CloseDoor = _ElectricDoor.close;

        if (Interior)  //在室內
        {
            distance = Vector3.Distance(Player.transform.position, Door.transform.position);  //離門的距離

            if (CloseDoor)  //門關上並在室內
            {
                Airtight = true;
                Pitch = 0.55f;
                AudioManager.StartLevelAudio();
            }
            else  
            {
                Airtight = false;
                switch (Deep)
                {
                    case 0:
                        Pitch = 1f;
                        break;
                    case 1:
                        Pitch = 0.9f;
                        break;
                    case 2:
                        Pitch = 0.8f;
                        break;
                }
                AudioManager.StartLevelAudio();
            }
            if (distance >= 13) //在室內深處
            {
                Deep = 2;
            }
            else if(distance >=4)
            {
                Deep = 1;
            }
            else
            {
                Deep = 0;
            }
        }
        else
        {
            Airtight = false;
            Pitch = 1; 
            AudioManager.StartLevelAudio();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                Interior = true;
                Player = other.gameObject;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                Interior = false;
            }
        }
    }
}
