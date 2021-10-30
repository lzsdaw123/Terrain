using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class CustomSceneManager : MonoBehaviour
{
    static CustomSceneManager instance;
    public static UnityAction onScenesLoadingEvent;
    public static UnityAction onScenesLoadedEvent;
    Scene activeScene;

    void Awake()
    {
        if (instance != null)
        {
            instance = this;

        } //Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public static void LoadScene(string sceneName)
    {
        instance.LoadingScene(sceneName);
    }
    public void LoadingScene(string sceneName)
    {
        StartCoroutine(LoadingRoutine(sceneName));
    }

    IEnumerator LoadingRoutine(string sceneName)
    {
        SceneManager.LoadScene("Messenger", LoadSceneMode.Additive);

        yield return null;

        onScenesLoadingEvent?.Invoke();
        SceneManager.UnloadSceneAsync(activeScene);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        yield return null;

        onScenesLoadedEvent?.Invoke();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        SceneManager.UnloadSceneAsync("Messenger");
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    CustomSceneManager.LoadScene("GamePlayScene");
        //    activeScene = SceneManager.GetActiveScene();
        //}
    }
}
