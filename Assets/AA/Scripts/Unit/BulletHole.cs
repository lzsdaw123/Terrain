using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float BulletHoleTime = 5f;  //彈孔持續時間

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletHoleTime -= Time.deltaTime;
        if (BulletHoleTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
