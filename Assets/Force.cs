using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public bool �}�� = false;
    Rigidbody[] rigidbodies = null;
    [Range(0, 50)]
    public float �}���O�D = 5;
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach(var r in rigidbodies) {
            r.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(�}��) {
            �}�� = false;
            foreach(var r in rigidbodies) {
                r.isKinematic = false;
                r.AddForce(new Vector3((Random.value*2-1)* �}���O�D,0, (Random.value * 2 - 1)* �}���O�D),ForceMode.Impulse);
            }
        }
    }
}
