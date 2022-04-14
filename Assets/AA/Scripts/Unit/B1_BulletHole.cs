using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_BulletHole : MonoBehaviour
{
    public float BulletHoleTime;  //彈孔持續時間
    public float[] InputTime;  //輸入生命時間
    public ObjectPool pool_Hit;
    public Animator ani;
    public int ButtleType; //武器類型
    [SerializeField] private GameObject Light;
    float LightRange;
    public bool AutoDead=true;
    public bool Dead;
    public GameObject father;

    // Start is called before the first frame update
    void Awake()
    {
        InputTime = new float[] { 5f, 3f, 5f };
    }
    void Start()
    {
        BulletHoleTime = InputTime[ButtleType];
        ani.SetInteger("Type", ButtleType);
        if (!AutoDead) BulletHoleTime = -1;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        if(Light.gameObject != null)
        {
            if (ButtleType == 1)
            {
                Light.GetComponent<Light>().range = 10;
                Light.SetActive(true);
            }
            else
            {
                Light.SetActive(false);
            }
        }
        father = transform.parent.gameObject;
        Dead = false;
    }

    void Update()
    {
        //transform.parent = gameObject.transform;
        if (ShootingRange.TargetWall && !Dead)
        {
            Dead = true;
            BulletHoleTime = -1;
        }
        else if (!ShootingRange.TargetWall && Dead)
        {
            Dead = false;
            BulletHoleTime = 0;
        }
        if (BulletHoleTime > 0)
        {
            BulletHoleTime -= Time.deltaTime;
        }
        if (BulletHoleTime <= 0 && BulletHoleTime>-1)
        {
            pool_Hit.RecoveryBoss1Hit(gameObject);     
        }
        if (Light.gameObject != null)
        {
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
    }
    void OnDisable()
    {
        if (Light.gameObject != null)
        {
            if (ButtleType == 1)
            {
                Light.GetComponent<Light>().range = 10;
                Light.SetActive(true);
            }
            else
            {
                Light.SetActive(false);
            }
        }
        BulletHoleTime = InputTime[ButtleType];
        if (!AutoDead) BulletHoleTime = -1;
        Dead = false;
    }
}
