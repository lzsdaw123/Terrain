using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float BulletHoleTime;  //彈孔持續時間
    public float[] InputTime;  //輸入生命時間
    public ObjectPool pool_Hit;
    [SerializeField] private int WeaponType; //武器類型
    [SerializeField] private GameObject Light;
    float LightRange;
    public bool AutoDead=true;
    public bool Dead;
    public GameObject father;
    public GameObject[] AwardHit; //擊中怪物的獎勵特效
    public GameObject PlayCam;
    public float distance;
    public Vector3 Size;
    public Vector3 NewSize;
    bool AutoSize;

    void Awake()
    {
        InputTime = new float[] { 5f, 3f, 5f };
    }
    void Start()
    {
        WeaponType = Shooting.WeaponType;
        BulletHoleTime = InputTime[WeaponType];
        if (!AutoDead) BulletHoleTime = -1;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        PlayCam = GameObject.Find("Gun_Camera").gameObject;
        if(Light.gameObject != null)
        {
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
        father = transform.parent.gameObject;
        Dead = false;
        AutoSize = true;
    }

    void Update()
    {
        if (AutoSize)
        {
            for (int i = 0; i < AwardHit.Length; i++)
            {
                if (AwardHit[i].activeSelf)
                {
                    distance = Vector3.Distance(transform.position, PlayCam.transform.position);  //彈孔與玩家距離
                    Size = new Vector3(0.8f, 0.8f, 0.8f);
                    if (distance >= 30)
                    {
                        float D = (distance - 30) * 0.15f;
                        if (D >= 4.5f) D = 4.5f;
                        Size *= D;
                        AwardHit[i].transform.localScale = Size;
                    }
                    else
                    {
                        AwardHit[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    }
                    NewSize = AwardHit[i].transform.localScale;
                    AutoSize = false;
                }
            }
        }
        
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
            pool_Hit.RecoveryHit(gameObject);     
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
        WeaponType = Shooting.WeaponType;
        if (Light.gameObject != null)
        {
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
        BulletHoleTime = InputTime[WeaponType];
        if (!AutoDead) BulletHoleTime = -1;
        Dead = false;
        AutoSize = true;
    }
}
