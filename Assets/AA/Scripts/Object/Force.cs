using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public static bool 破門 = false;
    Rigidbody[] rigidbodies = null;
    MeshCollider[] meshcollider = null;
    [Range(0, 50)]
    public float 破門力道;
    float time;
    bool isGrounded;
    float groundDistance = 5f;
    public LayerMask Ground;
    public GameObject G;

    void Start()
    {
        破門力道 = 2;
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        meshcollider = GetComponentsInChildren<MeshCollider>();
        //G = GetComponent<GameObject>();
        foreach (var r in rigidbodies) {
            r.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(破門) {
            time += 0.8f * Time.deltaTime;

            foreach (var r in rigidbodies)
            {
                r.isKinematic = false;
                r.AddForce(new Vector3((Random.value*2-1)* 破門力道,0, (Random.value * 2 - 1)* 破門力道),ForceMode.Impulse);
                if (time >= 5)
                {
                    if (isGrounded)
                    {
                        r.isKinematic = true;

                        foreach (var c in meshcollider)
                        {
                            c.isTrigger = true;
                            if (time >= 5.2f)
                            {
                                time = 5.2f;
                                r.isKinematic = false;
                            }
                        }
                        
                    }                               
                }
             }       
        }
        if (time >= 5.1f && !isGrounded)
        {
            G.SetActive(false);
        }
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, Ground);

    }
    public static void 開始破門()
    {
        破門 = true;
    }
}
