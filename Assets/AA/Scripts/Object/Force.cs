using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public int Type;
    public static bool �}��;
    [SerializeField] bool SF_�}��;
    [SerializeField] Rigidbody[] rigidbodies = null;
    [SerializeField] MeshCollider[] meshcollider = null;
    [Range(0, 50)]
    public float �}���O�D=2;
    float time;
    bool isGrounded;
    float groundDistance = 5f;
    public LayerMask Ground;
    public GameObject G;
    MeshCollider meshCollider;
    [SerializeField] CapsuleCollider CapsuleCollider=null;

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        //G = GetComponent<GameObject>();
        foreach (var r in rigidbodies) {
            r.isKinematic = true;
        }
        if (GetComponent<MeshCollider>())
        {
            meshcollider = GetComponentsInChildren<MeshCollider>();
            //meshCollider = GetComponent<MeshCollider>();
            //meshCollider.enabled = true;
        }
        CapsuleCollider = GetComponent<CapsuleCollider>();

    }

    // Update is called once per frame
    void Update()
    {

        SF_�}�� = �}��;

        if (�}��) {
            time += 0.8f * Time.deltaTime;
            foreach (var r in rigidbodies)
            {
                r.isKinematic = false;
                r.AddForce(new Vector3((Random.value*2-1)* �}���O�D,0, (Random.value * 2 - 1)* �}���O�D),ForceMode.Impulse);

                if (time >= 5)
                {
                    if (GetComponent<MeshCollider>())
                    {
                        //meshCollider.enabled = false;
                    }
                    if (isGrounded)
                    {
                        //r.isKinematic = true;

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
    public static void �}�l�}��()
    {
        �}�� = true;
    }
}
