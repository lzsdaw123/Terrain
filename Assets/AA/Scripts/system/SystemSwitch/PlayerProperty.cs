using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProperty : MonoBehaviour
{
    [SerializeField] int playerHealth = 100;
    [SerializeField] int playerDamage = 100;
    [SerializeField] int playerExprience = 100;
    [SerializeField] GameObject Setting;
    [SerializeField] GameObject AudioManager;

    void Start()
    {
        CustomSceneManager.onScenesLoadingEvent += PassDataToMessanger;
        Settings.onScenesLoadingEvent += PassDataToGameObject;
    }
    void OnDestroy()
    {
        CustomSceneManager.onScenesLoadingEvent -= PassDataToMessanger;
        Settings.onScenesLoadingEvent -= PassDataToGameObject;
    }
    void PassDataToMessanger()
    {
        SceneMessenger.instance.PassMessage("playerHealth", playerHealth.ToString());
        SceneMessenger.instance.PassMessage("playerDamage", playerDamage.ToString());
        SceneMessenger.instance.PassMessage("playerExprience", playerExprience.ToString());
    }
    void PassDataToGameObject()
    {
        SceneMessenger.instance.Passobjects("Setting", Setting);
        SceneMessenger.instance.Passobjects("AudioManager", AudioManager);
        
    }
}
