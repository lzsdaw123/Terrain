using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ObjectPool : MonoBehaviour
{
    public  GameObject Bullet, Hit;
    //物件池集中位置
    public GameObject BulletPool, HitPool, MBulletPool, MonsterPool_A, MonsterPool_B, MonsterPool_C,
        B1_BulletPool, B1_HitPool, B2_BulletPool, B2_HitPool; 
    public GameObject[] Monster;    // 可生的怪種類
    public MonsterAttributes[] MonsterAttributes = new MonsterAttributes[3];  //怪物尺寸
    public GameObject MBullet ;	// 怪物子彈
    public GameObject B1_Bullet ;	// 水晶BOSS子彈
    public GameObject B2_Bullet ;	// 機械BOSS子彈
    public GameObject B1_Hit; 	//水晶BOSS彈孔
    public GameObject B2_Hit; 	//機械BOSS彈孔
    public SpawnRay _SpawnRay;

    public int inttailSize;  //預置物件數量
    public int[] inttailSizeMS;  //怪物預置物件數量
    Vector3 muzzlePOS;

    private int[] uid =new int[] { 0, 0, 0 };								// 怪物編號


    private Queue<GameObject> _pool = new Queue<GameObject>();
    private Queue<GameObject> _pool_Hit = new Queue<GameObject>();

    private Queue<GameObject> Monster_poolA = new Queue<GameObject>();
    private Queue<GameObject> Monster_poolB = new Queue<GameObject>();
    private Queue<GameObject> Monster_poolC = new Queue<GameObject>();
    private Queue<GameObject> M_Bullet_pool = new Queue<GameObject>();
    private Queue<GameObject> B1_Bullet_pool = new Queue<GameObject>();
    private Queue<GameObject> B2_Bullet_pool = new Queue<GameObject>();
    private Queue<GameObject> B1_Hit_pool = new Queue<GameObject>();
    private Queue<GameObject> B2_Hit_pool = new Queue<GameObject>();

    public static float Harm;
    public static int SceneNub;
    public static bool color;

    void Awake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            UnityEditor.SceneVisibilityManager.instance.Show(gameObject, false);
#endif
        DontDestroyOnLoad(gameObject);  //切換場景時保留

        inttailSize = 8;  //物件池大小
        inttailSizeMS = new int[] { 16, 12, 8 };  //怪物物件池大小
        MonsterAttributes[0] = new MonsterAttributes(0.6f, 1.1f);
        MonsterAttributes[1] = new MonsterAttributes(1, 1.5f);
        MonsterAttributes[2] = new MonsterAttributes(0.2f, 0.6f);

        for (int cut =0;cut< inttailSize; cut++)
        {
            //GameObject go = Instantiate(Bullet) as GameObject;
            GameObject go = Instantiate(Bullet, BulletPool.transform) as GameObject; //生成子彈於子彈池
            GameObject go2 = Instantiate(Hit, HitPool.transform) as GameObject;   //生成彈孔於彈孔池
            GameObject Mo1B = Instantiate(MBullet, MBulletPool.transform) as GameObject;   //怪物子彈於怪物子彈池
            GameObject Boss1B = Instantiate(B1_Bullet, B1_BulletPool.transform) as GameObject;   //Boss1子彈於怪物子彈池
            GameObject Boss1BHit = Instantiate(B1_Hit, B1_HitPool.transform) as GameObject;   //Boss1彈孔於怪物子彈池
            GameObject Boss2B = Instantiate(B2_Bullet, B2_BulletPool.transform) as GameObject;   //Boss2子彈於怪物子彈池
            GameObject Boss2BHit = Instantiate(B2_Hit, B2_HitPool.transform) as GameObject;   //Boss2彈孔於怪物子彈池

            _pool.Enqueue(go);  //Queue.Enqueue() 將物件放入結構中
            _pool_Hit.Enqueue(go2);  //Queue.Enqueue() 將物件放入結構中

            M_Bullet_pool.Enqueue(Mo1B);  //Queue.Enqueue() 將怪物1子彈放入結構中
            B1_Bullet_pool.Enqueue(Boss1B);  //Queue.Enqueue() 將Boss1子彈放入結構中
            B2_Bullet_pool.Enqueue(Boss2B);  //Queue.Enqueue() 將Boss2子彈放入結構中
            B1_Hit_pool.Enqueue(Boss1BHit);  //Queue.Enqueue() 將Boss1彈孔放入結構中
            B2_Hit_pool.Enqueue(Boss2BHit);  //Queue.Enqueue() 將Boss2彈孔放入結構中
            go.SetActive(false);
            go2.SetActive(false);
            Mo1B.SetActive(false);
            Boss1B.SetActive(false);
            Boss1BHit.SetActive(false);
            Boss2B.SetActive(false);
            Boss2BHit.SetActive(false);
        }
        for (int cut = 0; cut < inttailSizeMS[0]; cut++)
        {
            //int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo1 = Instantiate(Monster[0], MonsterPool_A.transform) as GameObject;   //生成怪物於怪物池
            uid[0]++;                                      // 編號加1

            if (!Mo1.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo1.AddComponent<SpawnRayReg>();
            Mo1.SendMessage("Init", new MonterInfo(uid[0], _SpawnRay, 0));
            

            Monster_poolA.Enqueue(Mo1);  //Queue.Enqueue() 將怪物1放入結構中

            Mo1.SetActive(false);
        }
        for (int cut = 0; cut < inttailSizeMS[1]; cut++)  // 02
        {
            GameObject Mo2 = Instantiate(Monster[1], MonsterPool_B.transform) as GameObject;   //生成怪物於怪物池
            uid[1]++;                                      // 編號加1

            if (!Mo2.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo2.AddComponent<SpawnRayReg>();
            Mo2.SendMessage("Init", new MonterInfo(uid[1], _SpawnRay, 1));

            Monster_poolB.Enqueue(Mo2);  //Queue.Enqueue() 將怪物2放入結構中

            Mo2.SetActive(false);
        }
        for (int cut = 0; cut < inttailSizeMS[2]; cut++)  // 03
        {
            GameObject Mo3 = Instantiate(Monster[2], MonsterPool_C.transform) as GameObject;   //生成怪物於怪物池
            uid[2]++;                                      // 編號加1

            if (!Mo3.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo3.AddComponent<SpawnRayReg>();
            Mo3.SendMessage("Init", new MonterInfo(uid[2], _SpawnRay, 2));

            Monster_poolC.Enqueue(Mo3);  //Queue.Enqueue() 將怪物3放入結構中

            Mo3.SetActive(false);
        }
    }
    void Start()
    {
        color = false;
        MBulletPool.SetActive(true);
        MonsterPool_A.SetActive(true);
        MonsterPool_B.SetActive(true);
        MonsterPool_C.SetActive(true);
    }
    void Update()
    {
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub == 1 || PlayerResurrection.ReDelete)
        {
            Destroy(gameObject);
        }
        if (SceneNub == 3 || color)
        {
            color = false;
            MBulletPool.SetActive(false);
            MonsterPool_A.SetActive(false);
            MonsterPool_B.SetActive(false);
            MonsterPool_C.SetActive(false);
        }
    }
    //子彈
    public void ReUse (Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        if (_pool.Count > 0)
        {
            GameObject reuse = _pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;         
            reuse.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(Bullet, BulletPool.transform) as GameObject;
            go.transform.position = positon;
            go.transform.rotation = rotation;
        }
    }
    public void Recovery( GameObject recovery)  //用來回收物件
    {
        _pool.Enqueue(recovery);
        recovery.SetActive(false);
    }
    public static void harm(float power)
    {
        Harm = power;
    }
    //彈孔
    public void ReUseHit(Vector3 positon, Quaternion rotation,int HitType)  //取出存放在物件池中的物件
    {
        if (_pool_Hit.Count > 0)
        {
            GameObject reuse = _pool_Hit.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            GameObject reHit;
            reHit = reuse.transform.GetChild(HitType).gameObject;
            reuse.GetComponent<BulletHole>().HoleType = HitType;
            reuse.GetComponent<BulletHole>().power = Harm;
            reHit.SetActive(true);
            reuse.SetActive(true);
        }
        else
        {
            GameObject go2 = Instantiate(Hit, HitPool.transform) as GameObject;
            go2.transform.position = positon;
            go2.transform.rotation = rotation;
            GameObject reHit;
            reHit = go2.transform.GetChild(HitType).gameObject;
            go2.GetComponent<BulletHole>().HoleType = HitType;
            go2.GetComponent<BulletHole>().power = Harm;
            reHit.SetActive(true);
        }
    }
    public void RecoveryHit(GameObject recovery)  //用來回收物件
    {
        _pool_Hit.Enqueue(recovery);
        GameObject reHit;
        for (int i = 0; i < recovery.transform.childCount; i++)
        {
            reHit = recovery.transform.GetChild(i).gameObject;
            reHit.SetActive(false);
        }
        recovery.SetActive(false);
    }

    //怪物1
    public void ReUseMonster01(Vector3 positon, Quaternion rotation)  //取出存放在怪物池01中的怪物01
    {
        float Size = Random.Range(MonsterAttributes[0].MinSize, MonsterAttributes[0].MaxSize);  //怪物01大小0.6~ 1.1

        if (Monster_poolA.Count > 0)
        {
            GameObject reuse = Monster_poolA.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.transform.localScale = new Vector3(Size, Size, Size);  //怪物隨機大小
            reuse.SetActive(true);
        }
        else
        {
            //int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo1 = Instantiate(Monster[0], MonsterPool_A.transform) as GameObject;  //生成怪物於怪物池
            uid[0]++;                                      // 編號加1

            if (!Mo1.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo1.AddComponent<SpawnRayReg>();
            Mo1.SendMessage("Init", new MonterInfo(uid[0], _SpawnRay, 0));
            Mo1.transform.position = positon;
            Mo1.transform.rotation = rotation;
            Mo1.transform.localScale = new Vector3(Size, Size, Size);  //怪物隨機大小
            Monster_poolA.Enqueue(Mo1);  //Queue.Enqueue() 將怪物1放入結構中
        }
    }
    public void RecoveryMonster01(GameObject recovery)  //用來回收物件
    {
        Monster_poolA.Enqueue(recovery);
        recovery.SetActive(false);
    }
    //怪物2
    public void ReUseMonster02(Vector3 positon, Quaternion rotation)  //取出存放在怪物池02中的怪物02
    {
        float Size = Random.Range(MonsterAttributes[1].MinSize, MonsterAttributes[1].MaxSize);  //怪物02大小1~ 1.5

        if (Monster_poolB.Count > 0)
        {
            GameObject reuse = Monster_poolB.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.transform.localScale = new Vector3(Size, Size, Size + 0.1f);  //怪物隨機大小
            reuse.SetActive(true);
        }
        else
        {
            //int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo2 = Instantiate(Monster[1], MonsterPool_B.transform) as GameObject;  //生成怪物於怪物池
            uid[1]++;                                      // 編號加1

            if (!Mo2.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo2.AddComponent<SpawnRayReg>();
            Mo2.SendMessage("Init", new MonterInfo(uid[1], _SpawnRay, 1));
            Mo2.transform.position = positon;
            Mo2.transform.rotation = rotation;
            Mo2.transform.localScale = new Vector3(Size, Size, Size + 0.1f);  //怪物隨機大小
            Monster_poolB.Enqueue(Mo2);  //Queue.Enqueue() 將怪物2放入結構中
        }
    }
    public void RecoveryMonster02(GameObject recovery)  //用來回收物件
    {
        Monster_poolB.Enqueue(recovery);
        recovery.SetActive(false);
    }
    //怪物3
    public void ReUseMonster03(Vector3 positon, Quaternion rotation)  //取出存放在怪物池03中的怪物03
    {
        float Size = Random.Range(MonsterAttributes[2].MinSize, MonsterAttributes[2].MaxSize);  //怪物03大小1~ 1.5

        if (Monster_poolC.Count > 0)
        {
            GameObject reuse = Monster_poolC.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.transform.localScale = new Vector3(Size, Size, Size + 0.1f);  //怪物隨機大小
            reuse.SetActive(true);
        }
        else
        {
            //int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo3 = Instantiate(Monster[2], MonsterPool_C.transform) as GameObject;  //生成怪物於怪物池
            uid[2]++;                                      // 編號加1

            if (!Mo3.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo3.AddComponent<SpawnRayReg>();
            Mo3.SendMessage("Init", new MonterInfo(uid[2], _SpawnRay, 2));
            Mo3.transform.position = positon;
            Mo3.transform.rotation = rotation;
            Mo3.transform.localScale = new Vector3(Size, Size, Size + 0.1f);  //怪物隨機大小
            Monster_poolC.Enqueue(Mo3);  //Queue.Enqueue() 將怪物3放入結構中
        }
    }
    public void RecoveryMonster03(GameObject recovery)  //用來回收物件
    {
        Monster_poolC.Enqueue(recovery);
        recovery.SetActive(false);
    }

    //怪物1 子彈
    public void ReUseM01Bullet(Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        if (M_Bullet_pool.Count > 0)
        {
            GameObject reuse = M_Bullet_pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        else
        {
            GameObject Mo1B = Instantiate(MBullet, MBulletPool.transform) as GameObject;  //怪物子彈於怪物子彈池
            Mo1B.transform.position = positon;
            Mo1B.transform.rotation = rotation;
        }
    }
    public void RecoveryM01Bullet(GameObject recovery)  //用來回收物件
    {
        M_Bullet_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }

    //Boss1 子彈
    public void ReUseBoss1Bullet(Vector3 positon, Quaternion rotation,int BulletType, int Muzzle)  //取出存放在物件池中的物件
    {
        if (B1_Bullet_pool.Count > 0)
        {
            GameObject reuse = B1_Bullet_pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.GetComponent<B1_BulletLife>().BulletType = BulletType;
            reuse.GetComponent<B1_BulletLife>().cuMuGrid = Muzzle;
            reuse.SetActive(true);
        }
        else
        {
            GameObject Boss1B = Instantiate(B1_Bullet, B1_BulletPool.transform) as GameObject;  //Boss1子彈於怪物子彈池
            Boss1B.transform.position = positon;
            Boss1B.transform.rotation = rotation;
            Boss1B.GetComponent<B1_BulletLife>().BulletType = BulletType;
            Boss1B.GetComponent<B1_BulletLife>().cuMuGrid = Muzzle;
        }
    }
    public void RecoveryBoss1Bullet(GameObject recovery)  //用來回收物件
    {
        B1_Bullet_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }
    //Boss1 彈孔
    public void ReUseBoss1Hit(Vector3 positon, Quaternion rotation, int HitType)  //取出存放在物件池中的物件
    {
        if (B1_Hit_pool.Count > 0)
        {
            GameObject reuse = B1_Hit_pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.GetComponent<B1_BulletHole>().BulletType = HitType;
            reuse.GetComponent<B1_BulletHole>().ani.SetInteger("Type", HitType);
            //print(HitType+"  HitType   " + reuse.GetComponent<B1_BulletHole>().ButtleType);
            reuse.SetActive(true);
        }
        else
        {
            GameObject Boss1BHit = Instantiate(B1_Hit, B1_HitPool.transform) as GameObject;  //Boss1彈孔於怪物子彈池
            Boss1BHit.transform.position = positon;
            Boss1BHit.transform.rotation = rotation;
            Boss1BHit.GetComponent<B1_BulletHole>().BulletType = HitType;
            Boss1BHit.GetComponent<B1_BulletHole>().ani.SetInteger("Type", HitType);
        }
    }
    public void RecoveryBoss1Hit(GameObject recovery)  //用來回收物件
    {
        B1_Hit_pool.Enqueue(recovery);
        //GameObject reHit;
        //for (int i = 0; i < recovery.transform.childCount; i++)
        //{
        //    reHit = recovery.transform.GetChild(i).gameObject;
        //    reHit.SetActive(false);
        //}
        recovery.SetActive(false);
    }
    //Boss2 子彈
    public void ReUseBoss2Bullet(Vector3 positon, Quaternion rotation, int BulletType, int Muzzle)  //取出存放在物件池中的物件
    {
        if (B2_Bullet_pool.Count > 0)
        {
            GameObject reuse = B2_Bullet_pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = Vector3.zero;
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.GetComponent<S_BulletLife>().BulletType = BulletType;
            reuse.GetComponent<S_BulletLife>().Start();
            //reuse.GetComponent<S_BulletLife>().cuMuGrid = Muzzle;
            reuse.SetActive(true);
        }
        else
        {
            GameObject Boss2B = Instantiate(B2_Bullet, B2_BulletPool.transform) as GameObject;  //Boss1子彈於怪物子彈池
            Boss2B.transform.position = positon;
            Boss2B.transform.rotation = rotation;
            Boss2B.GetComponent<S_BulletLife>().BulletType = BulletType;
            Boss2B.GetComponent<S_BulletLife>().Start();
            //Boss1B.GetComponent<S_BulletLife>().cuMuGrid = Muzzle;
        }
    }
    public void RecoveryBoss2Bullet(GameObject recovery)  //用來回收物件
    {
        B2_Bullet_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }
    //Boss2 彈孔
    public void ReUseBoss2Hit(Vector3 positon, Quaternion rotation, int HitType)  //取出存放在物件池中的物件
    {
        if (B2_Hit_pool.Count > 0)
        {
            GameObject reuse = B2_Hit_pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.GetComponent<B2_BulletHole>().BulletType = HitType;
            GameObject reHit = reuse.transform.GetChild(HitType).gameObject;
            reHit.SetActive(true);
            //reuse.GetComponent<B2_BulletHole>().ani.SetInteger("Type", HitType);
            //print(HitType+"  HitType   " + reuse.GetComponent<B1_BulletHole>().ButtleType);
            reuse.SetActive(true);
        }
        else
        {
            GameObject Boss2BHit = Instantiate(B2_Hit, B2_HitPool.transform) as GameObject;  //Boss2彈孔於怪物子彈池
            Boss2BHit.transform.position = positon;
            Boss2BHit.transform.rotation = rotation;
            Boss2BHit.GetComponent<B2_BulletHole>().BulletType = HitType;
            GameObject reHit = Boss2BHit.transform.GetChild(HitType).gameObject;
            reHit.SetActive(true);
            //Boss2BHit.GetComponent<B2_BulletHole>().ani.SetInteger("Type", HitType);
        }
    }
    public void RecoveryBoss2Hit(GameObject recovery)  //用來回收物件
    {
        B2_Hit_pool.Enqueue(recovery);
        GameObject reHit;
        for (int i = 0; i < recovery.transform.childCount; i++)
        {
            reHit = recovery.transform.GetChild(i).gameObject;
            reHit.SetActive(false);
        }
        recovery.SetActive(false);
    }
}
