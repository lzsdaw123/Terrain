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
    public GameObject[] Hit;
    [SerializeField] private GameObject Light;
    float LightRange;
    public bool AutoDead=true;
    public bool Dead;
    public GameObject father;
    public GameObject[] clusterBomb;
    public bool clusterBombExp;
    public bool PlayAni;

    void Awake()
    {
        InputTime = new float[] { 5f, 3f, 5f };
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
    }
    void Start()
    {
        BulletHoleTime = InputTime[ButtleType];
        if (!AutoDead) BulletHoleTime = -1;
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
        //father = transform.parent.gameObject;
        Dead = false;
        //print("ani  "+ButtleType);
        ani.SetInteger("Type", ButtleType);
    }

    void Update()
    {
        if (!PlayAni)
        {
            PlayAni = true;
            ani.SetInteger("Type", ButtleType);
        }
        //transform.parent = gameObject.transform;
        if (clusterBombExp)  //子水晶爆炸
        {
            for(int i=0; i< clusterBomb.Length; i++)
            {
                clusterBomb[i].GetComponent<clusterBomb_Lift>().StartAttack = true;
            }
        }

        if (BulletHoleTime > 0)  //開始死亡倒數
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
    public void Generate(int Type)
    {
        switch (Type)
        {
            case 0:
                clusterBombExp = true;
                BulletHoleTime = InputTime[ButtleType];
                break;
            case 1:
                BulletHoleTime = InputTime[ButtleType];
                break;
            case 2:
                BulletHoleTime = InputTime[ButtleType];
                break;
        }
    } 
    void OnDisable()
    {
        Hit[0].SetActive(false);
        Hit[1].SetActive(false);
        Hit[2].SetActive(false);
        PlayAni = false;
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
        ani.enabled = true;
        clusterBombExp = false;
        for (int i = 0; i < clusterBomb.Length; i++)
        {
            clusterBomb[i].GetComponent<clusterBomb_Lift>().StartAttack = false;
            clusterBomb[i].SetActive(true);
        }
    }
}
