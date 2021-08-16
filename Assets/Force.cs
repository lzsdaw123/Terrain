using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public bool 破門 = false;
    Rigidbody[] rigidbodies = null;
    [Range(0, 50)]
    public float 破門力道 = 5;
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
        if(破門) {
            破門 = false;
            foreach(var r in rigidbodies) {
                r.isKinematic = false;
                r.AddForce(new Vector3((Random.value*2-1)* 破門力道,0, (Random.value * 2 - 1)* 破門力道),ForceMode.Impulse);
            }
        }
    }
}
