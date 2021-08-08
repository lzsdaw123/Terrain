using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public GameObject Enter, Exit;
    Vector3 Exut_P;
    public float X;

    void Start()
    {
        Exut_P= Exit.transform.position;
        Exut_P.x += X;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.position = Exut_P;
        }
    }
}
