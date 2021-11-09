using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject MianCamera;
    public Vector3 CameraPos;
    public Transform MC_1, MC_2;
    public bool Move;

    //Vector3 targetPosition = new Vector3(0, 0, 10);   // 目標位置
    Vector3 currentVelocity = Vector3.zero;     // 當前速度，這個值由你每次呼叫這個函式時被修改
    float maxSpeed = 5f;    // 選擇允許你限制的最大速度
    float smoothTime = 5f;      // 達到目標大約花費的時間。 一個較小的值將更快達到目標。

    //Vector3 targetPosition = new Vector3(0, 0, 10);
    float maxDistanceDelta = 1f;
    // maxDistanceDelta的負值從目標推開向量，就是說maxDistanceDelta是正值，當前地點移向目標，如果是負值當前地點將遠離目標。

    void Start()
    {
        //MianCamera.SetActive(false);
        //MianCamera.transform.position = MC_1.position;
        CameraPos = MC_1.position;
        Move = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            Move = true;
        }
        if(Move)
        {
            //MianCamera.SetActive(true);
            //MianCamera.transform.position = Vector3.SmoothDamp(transform.position, MC_2.position, ref currentVelocity, smoothTime, maxSpeed);
            //MianCamera.transform.position = Vector3.MoveTowards(transform.position, MC_2.position, maxDistanceDelta);
        }
    }
}
