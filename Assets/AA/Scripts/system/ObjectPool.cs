using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public  GameObject Bullet, Hit;
    public  GameObject BulletPool, HitPool;
    public int inttailSize;
    Vector3 muzzlePOS;

    private Queue<GameObject> _pool = new Queue<GameObject>();
    private Queue<GameObject> _pool_Hit = new Queue<GameObject>();

    void Awake()
    {
        inttailSize = 12;  //物件池大小

        for (int cut =0;cut< inttailSize; cut++)
        {
            //GameObject go = Instantiate(Bullet) as GameObject;
            GameObject go = Instantiate(Bullet, BulletPool.transform) as GameObject; //生成子彈於子彈池
            GameObject go2 = Instantiate(Hit, HitPool.transform) as GameObject;   //生成彈孔於彈孔池

            _pool.Enqueue(go);  //Queue.Enqueue() 將物件放入結構中
            _pool_Hit.Enqueue(go2);  //Queue.Enqueue() 將物件放入結構中
            go.SetActive(false);
            go2.SetActive(false);                       
        }
    }
    //子彈
    public void ReUse (Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        muzzlePOS = positon;
        if (_pool.Count > 0)
        {
            GameObject reuse = _pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;         
            reuse.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(Bullet) as GameObject;
            go.transform.position = positon;
            go.transform.rotation = rotation;
        }
    }
    public void Recovery( GameObject recovery)  //用來回收物件
    {
        _pool.Enqueue(recovery);
        recovery.SetActive(false);
        recovery.transform.position = muzzlePOS;
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
            reHit.SetActive(true);
            reuse.SetActive(true);
        }
        else
        {
            GameObject go2 = Instantiate(Hit) as GameObject;
            go2.transform.position = positon;
            go2.transform.rotation = rotation;
            GameObject reHit;
            reHit = go2.transform.GetChild(HitType).gameObject;
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


}
