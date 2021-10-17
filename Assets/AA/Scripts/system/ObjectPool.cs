using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public  GameObject prefab;
    public  GameObject BS;
    public int inttailSize;

    private Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        inttailSize = 3;  //物件池大小

        for (int cut =0;cut< inttailSize; cut++)
        {
            GameObject go = Instantiate(prefab) as GameObject;
            //GameObject go = Instantiate(prefab,BS.transform) as GameObject;
            _pool.Enqueue(go);  //Queue.Enqueue() 將物件放入結構中
            go.SetActive(false);                       
        }
    }

    public void ReUse (Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        print(_pool.Count);
        if (_pool.Count > 0)
        {
            GameObject reuse = _pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
            //print(_pool.Dequeue());
        }
        else
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.transform.position = positon;
            go.transform.rotation = rotation;
        }
    }
    public void Recovery(GameObject recovery)  //用來回收物件
    {
        _pool.Enqueue(recovery);
        //print(recovery);
        recovery.SetActive(false);
    }


}
