using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster01 : MonoBehaviour
{
    public float hpFull = 5;
    public float hp;

    void Start()
    {
        hp = hpFull;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damage(float Power)
    {
        hp -= Power;
        if (hp <= 0)
        {
            hp = 0;
            Destroy(gameObject);
        }
    }
}
