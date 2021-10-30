using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSC : MonoBehaviour
{
    Transform parent;

    //接收
    void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
    }
    void Start()
    {
        CustomSceneManager.onScenesLoadedEvent += LoadPlayProterties;
        Settings.onScenesLoadedEvent += LoadPlayProterties;
    }
    void OnDestroy()
    {
        CustomSceneManager.onScenesLoadedEvent -= LoadPlayProterties;
        Settings.onScenesLoadedEvent -= LoadPlayProterties;
    }

    void LoadPlayProterties()
    {
        string playerHealth = SceneMessenger.instance.GetMessage("playerHealth");
        string playerDamage = SceneMessenger.instance.GetMessage("playerDamage");
        string playerExprience = SceneMessenger.instance.GetMessage("playerExprience");

        GameObject Settings = SceneMessenger.instance.Getobjects("Settings", transform);
        GameObject AudioManager = SceneMessenger.instance.Getobjects("AudioManager", transform);

        //Debug.Log("h__" + playerHealth);
        //Debug.Log("d__" + playerDamage);
        //Debug.Log("e__" + playerExprience);
        Debug.Log("物件Settings+" + Settings);
        Debug.Log("物件AudioManager+" + AudioManager);
    }
}
