using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float BulletHoleTime;  //彈孔持續時間
    public float[] InputTime;
    public ObjectPool pool_Hit;
    [SerializeField] private int WeaponType; //武器類型
    [SerializeField] private GameObject Light;
    float LightRange;


    // Start is called before the first frame update
    void Awake()
    {
        InputTime = new float[] { 5f, 3f, 5f };
    }
    void Start()
    {
        WeaponType = Shooting.WeaponType;
        BulletHoleTime = InputTime[WeaponType];
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        if (WeaponType == 1)
        {
            Light.GetComponent<Light>().range = 10;
            Light.SetActive(true);
        }
        else
        {
            Light.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        BulletHoleTime -= Time.deltaTime;
        if (BulletHoleTime <= 0)
        {
            pool_Hit.RecoveryHit(gameObject);     
        }
        if (Light.activeSelf)
        {
            Light.GetComponent<Light>().range -= 16 * Time.deltaTime;
            LightRange = Light.GetComponent<Light>().range;

            if (LightRange <= 0)
            {
                Light.GetComponent<Light>().range = 0;
                Light.SetActive(false);
            }
        }
    }
    void OnDisable()
    {
        WeaponType = Shooting.WeaponType;
        if (WeaponType == 1)
        {
            Light.GetComponent<Light>().range = 10;
            Light.SetActive(true);
        }
        else
        {
            Light.SetActive(false);
        }
        BulletHoleTime = InputTime[WeaponType];
    }
}
