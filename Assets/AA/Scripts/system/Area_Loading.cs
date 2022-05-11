using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area_Loading : MonoBehaviour
{
    public GameObject[] Research_Room; //��s��
    public GameObject[] Mine; //�q�|
    public static int Type;
    public static bool Load;

    void Start()
    {
        Load = false;
        for (int i = 0; i < Research_Room.Length; i++)
        {
            Research_Room[i].SetActive(false);
        }
        for (int i = 0; i < Mine.Length; i++)
        {
            Mine[i].SetActive(false);
        }
    }

    void Update()
    {
        if (Load)
        {
            switch (Type)
            {
                case 0:
                    for(int i=0; i< Research_Room.Length; i++)
                    {
                        Research_Room[i].SetActive(true);
                    }
                    break;
                case 1:
                    for (int i = 0; i < Mine.Length; i++)
                    {
                        Mine[i].SetActive(true);
                    }
                    break;
            }
            Load = false;
        }
    }
    public static void AreaLoading(int type)
    {
        Type = type;
        Load = true;
    }
}
//[Serializable]
//public class Area
//{
//    public GameObject[] Research_Room; //��s��
//    public GameObject[] Mine; //�q�|


//    /// <summary>
//    /// �U�Z���ƭ�
//    /// </summary>

//    /// <returns></returns>
//    public Area(GameObject[] research_Room, GameObject[] mine)
//    {
//        Research_Room = research_Room;
//        Mine = mine;
//    }
//}
