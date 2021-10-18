using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float BulletHoleTime;  //彈孔持續時間
    public ObjectPool pool_Hit;


    // Start is called before the first frame update
    void Start()
    {
        BulletHoleTime = 5f;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();

    }

    // Update is called once per frame
    void Update()
    {
        BulletHoleTime -= Time.deltaTime;
        if (BulletHoleTime <= 0)
        {
            pool_Hit.RecoveryHit(gameObject);
            BulletHoleTime = 5f;
        }
    }
}
