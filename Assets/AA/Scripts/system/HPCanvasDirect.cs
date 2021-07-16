using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPCanvasDirect : MonoBehaviour
{
	private Transform camTrans;  //攝影機的transform

    void Start()
    {
		camTrans = Camera.main.transform;
    }

    void Update()
    {
		transform.rotation = camTrans.rotation; //修正和攝影機同方向
    }
}
