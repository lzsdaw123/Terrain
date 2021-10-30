using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMessenger : MonoBehaviour
{
    public static SceneMessenger instance;
    [SerializeField] GameObject SetOb;
    [SerializeField] GameObject AMOb;
    void Awake()
    {
        instance = this;
    }
    Dictionary<string, string> messages = new Dictionary<string, string>();
    Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    public void Passobjects(string address, GameObject gameObject)  //傳遞地址 address
    {
        objects.Add(address, gameObject);
        gameObject.transform.parent = transform;
        if(address== "Setting")
        {
            SetOb = gameObject;
        }
        else
        {
            AMOb = gameObject;
        }
        
    }
    public GameObject Getobjects(string address, Transform parent)  //傳遞的訊息 message
    {
        objects.TryGetValue(address, out GameObject STO);
        objects.TryGetValue(address, out GameObject AMO);
        STO = SetOb;
        AMO = AMOb;
        gameObject.transform.parent = parent;
        return gameObject;
    }


    public void PassMessage(string address, string message)  //傳遞地址 address
    {
        messages.Add(address, message);
    }
    public string GetMessage(string address)  //傳遞的訊息 message
    {
        messages.TryGetValue(address, out string message);
        return message;
    }

}
