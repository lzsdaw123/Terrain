using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2_BulletHole : MonoBehaviour
{
    public float BulletHoleTime;  //彈孔持續時間
    public float[] InputTime;  //輸入生命時間
    public ObjectPool pool_Hit;
    public Animator ani;
    public int BulletType; //武器類型
    public GameObject[] Hit;
    [SerializeField] private GameObject Light;
    float LightRange;
    public bool AutoDead=true;
    public bool Dead;
    public GameObject father;
    public GameObject[] clusterBomb;
    public bool clusterBombExp;  //集束炸彈
    public bool PlayAni;

    void Awake()
    {
        InputTime = new float[] { 5f, 5f, 2f };
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
    }
    void Start()
    {
        BulletHoleTime = InputTime[BulletType];
        if (!AutoDead) BulletHoleTime = -1;
        if(Light.gameObject != null)
        {
            if (BulletType == 1)
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
        if (ani != null) ani.SetInteger("Type", BulletType);
    }

    void Update()
    {
        if (!PlayAni)
        {
            PlayAni = true;
            if (ani != null)  ani.SetInteger("Type", BulletType);
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
            pool_Hit.RecoveryBoss2Hit(gameObject);     
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
                BulletHoleTime = InputTime[BulletType];
                break;
            case 1:
                BulletHoleTime = InputTime[BulletType];
                break;
            case 2:
                BulletHoleTime = InputTime[BulletType];
                break;
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                other.gameObject.SendMessage("Damage", 0.1f); //傷害
                other.gameObject.SendMessage("DamageEffects", 4); //傷害特效
                other.gameObject.SendMessage("hit_Direction", transform); //命中方位
            }
        }
    }
    void OnDisable()
    {
        for(int h=0; h<Hit.Length; h++)
        {
            Hit[h].SetActive(false);
        }

        PlayAni = false;
        if (Light.gameObject != null)
        {
            if (BulletType == 1)
            {
                Light.GetComponent<Light>().range = 10;
                Light.SetActive(true);
            }
            else
            {
                Light.SetActive(false);
            }
        }
        BulletHoleTime = InputTime[BulletType];
        if (!AutoDead) BulletHoleTime = -1;
        Dead = false;
        if(ani !=null) ani.enabled = true;
        clusterBombExp = false;
        for (int i = 0; i < clusterBomb.Length; i++)
        {
            clusterBomb[i].GetComponent<clusterBomb_Lift>().StartAttack = false;
            clusterBomb[i].SetActive(true);
        }
    }
}
