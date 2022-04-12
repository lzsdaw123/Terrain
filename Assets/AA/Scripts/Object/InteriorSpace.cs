using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorSpace : MonoBehaviour
{
    public static bool Interior;  //�Ǥ�
    public static bool Airtight;  //�K��
    public static int Deep;  //�`�B
    [SerializeField] bool �Ǥ�;
    [SerializeField] bool �K��;
    [SerializeField] int �`�B;
    public GameObject Door;
    public ElectricDoor _ElectricDoor;
    [SerializeField] bool CloseDoor;
    public float distance;
    public GameObject Player;
    public static float Pitch;
    [SerializeField] float SF_Pitch;
    [SerializeField] float OirPitch;

    void Start()
    {
        Interior = false;
        Pitch = 1;
        OirPitch = 0;
    }

    void Update()
    {
        �Ǥ� = Interior;
        �K�� = Airtight;
        �`�B = Deep;
        SF_Pitch = Pitch;
        CloseDoor = _ElectricDoor.close;

        if (Interior)  //�b�Ǥ�
        {
            distance = Vector3.Distance(Player.transform.position, Door.transform.position);  //�������Z��

            if (CloseDoor)  //�����W�æb�Ǥ�
            {
                Airtight = true;
                Pitch = 0.55f;
                PlayAudio();
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
                PlayAudio();
            }
            if (distance >= 13) //�b�Ǥ��`�B
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
            PlayAudio();
        }
        if (OirPitch != Pitch)
        {
            AudioManager.StartLevelAudio();
            OirPitch = Pitch;
        }
    }
    void PlayAudio()
    {

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
