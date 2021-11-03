using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCanvasDiract : MonoBehaviour
{
    private Transform camTrans;
    public Camera Camera;

    void Start()
    {
        camTrans = Camera.transform;
    }

    void Update()
    {
        transform.rotation = camTrans.rotation;
    }
}
