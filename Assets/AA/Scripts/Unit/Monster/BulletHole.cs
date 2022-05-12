using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletHole : MonoBehaviour
{
    public int HoleType;  //彈孔類型
    public float BulletHoleTime;  //彈孔持續時間
    public float[] InputTime;  //輸入生命時間
    public ObjectPool pool_Hit;
    [SerializeField] private int WeaponType; //武器類型
    [SerializeField] private GameObject Light;
    float LightRange;
    public bool AutoDead=true;
    public bool Dead;
    public bool Move;
    public GameObject father;
    public GameObject[] AwardHit; //擊中怪物的獎勵特效
    public GameObject PlayCam;
    public float distance;
    public Vector3 Size;
    public Vector3 g_Size;
    public Vector3 NewSize;
    bool AutoSize;
    public float R_move;
    public float power;
    public Text powerText;
    Color Color;

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
        Dead = Move = false;
        AutoSize = true;
        R_move = Random.Range(-3, 3);
    }

    void Update()
    {
        if (AutoSize)
        {
            for (int i = 0; i < AwardHit.Length; i++)
            {
                if (AwardHit[i].activeSelf)
                {
                    switch (i)
                    {
                        case 0:
                        case 1:
                            Size = new Vector3(0.8f, 0.8f, 0.8f);
                            break;
                        case 2:
                            Size = new Vector3(2f, 2f, 2f);
                            break;
                    }
                    distance = Vector3.Distance(transform.position, PlayCam.transform.position);  //彈孔與玩家距離
                    if (distance >= 30)
                    {
                        float D = (distance - 30) * 0.15f;
                        if (D >= 4.5f) D = 4.5f;
                        Size *= D;
                        g_Size *= D;
                        AwardHit[i].transform.localScale = Size;
                    }
                    else  //最小尺寸
                    {
                        switch (i)
                        {
                            case 0:
                            case 1:
                                AwardHit[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                                break;
                            case 2:
                                AwardHit[i].transform.localScale = new Vector3(2f, 2f, 2f);
                                break;
                        }
                    }
                    NewSize = AwardHit[i].transform.localScale;
                    AutoSize = false;
                }
            }
        }

        //transform.parent = gameObject.transform;
        if (HoleType==7 && !Dead)
        {
            Dead = true;
            Move = true;
            BulletHoleTime = 1f;
            g_Size = new Vector3(2f, 2f, 2f);  //傷害文字初始尺寸
            int harm = (int) (power * 10);
            powerText.text = "" + harm;
            Color = new Color(1, 0, 0, 1);
            powerText.color = Color;
        }

        if (ShootingRange.TargetWall && !Dead)  //進入靶場
        {
            //BulletHoleTime = -1;
            //Dead = true;
            //Move = true;
            //BulletHoleTime = 1;
            //R_move = Random.Range(-1, 1);
        }
        else if (!ShootingRange.TargetWall && Dead)  //離開靶場
        {
            //Dead = false;
            //Move = false;
            //BulletHoleTime = 0;
        }
        if (BulletHoleTime > 0)
        {
            BulletHoleTime -= Time.deltaTime;
            if (Move)
            {
                AwardHit[2].transform.localPosition += new Vector3(R_move, 0, 2f) * Time.deltaTime;
                g_Size -= new Vector3(1f, 1f, 1f)*2f * Time.deltaTime;
                if (g_Size.x <= 0.5f) g_Size = new Vector3(0.5f, 0.5f, 0.5f);
                AwardHit[2].transform.GetChild(1).transform.localScale = g_Size;
                Color.a -= 0.5f * Time.deltaTime;
                powerText.color = Color;
            }
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
        AwardHit[2].transform.localPosition = Vector3.zero;
        BulletHoleTime = InputTime[WeaponType];
        if (!AutoDead) BulletHoleTime = -1;
        Dead = Move = false;
        AutoSize = true;
    }
}
