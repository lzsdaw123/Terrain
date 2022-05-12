using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCanvasDiract : MonoBehaviour
{
    private Transform camTrans;
    public Camera Camera;

    void Start()
    {
        Camera = Save_Across_Scene.Gun_Camera;
        camTrans = Camera.transform;
    }

    void Update()
    {
        if (Camera != null)
        {
            transform.rotation = camTrans.rotation;
        }
    }
}
