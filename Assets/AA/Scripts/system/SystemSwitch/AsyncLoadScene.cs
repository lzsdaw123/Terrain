using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Globe
{
    public static string nextSceneName;
}

public class AsyncLoadScene : MonoBehaviour
{
	public Scrollbar loadingScrollbar;  //讀取條
	public Text loadingText;   //讀取%數
	private float loadingSpeed = 1;
	private float targetValue;
	private AsyncOperation operation;
	public GameObject AnyKey;
	float time;
	bool AnyKetB;
	public HeroLife heroLife;
	public Shooting shooting;

	void Start()
	{
		loadingScrollbar.size = 0.0f;
		Time.timeScale = 1f;
		time = 0;
		AnyKetB = false;
		AnyKey.SetActive(false);

		if (SceneManager.GetActiveScene().name == "Messenger")
		{
			//啟動協程
			StartCoroutine(AsyncLoading());
		}
	}

	IEnumerator AsyncLoading()
	{
		operation = SceneManager.LoadSceneAsync(Globe.nextSceneName);
		//阻止當載入完成自動切換
		operation.allowSceneActivation = false;

		yield return operation;
	}

	void Update()
	{
		targetValue = operation.progress;
        //Level_1.MissionTime = -1;
        //Level_1.UiOpen = false;
        PlayerView.UI_Stop = true;

		if (operation.progress >= 0.9f)
		{
			//operation.progress的值最大為0.9
			targetValue = 1.0f;
		}
        if (AnyKetB)
		{
			time +=2 * Time.deltaTime;
            if (time < 1)
            {
				AnyKey.SetActive(true);
			}
			if(time >= 1)
            {			
				AnyKey.SetActive(false);
			}
			if(time>=2) time = 0;
		}
		if (targetValue != loadingScrollbar.size)
		{
			//插值運算
			loadingScrollbar.size = Mathf.Lerp(loadingScrollbar.size, targetValue, Time.deltaTime * loadingSpeed);
			if (Mathf.Abs(loadingScrollbar.size - targetValue) < 0.01f)
			{
				loadingScrollbar.size = targetValue;
			}
		}

		loadingText.text = ((int)(loadingScrollbar.size * 100)).ToString() + "%";

		if ((int)(loadingScrollbar.size * 100) == 100)
		{
			AnyKetB = true;
			//AnyKey.SetActive(true);
			if (Input.anyKeyDown)
            {
				AnyKey.SetActive(false);
				//允許非同步載入完畢後自動切換場景
				operation.allowSceneActivation = true;
				PlayerResurrection.ReO = true;
				PlayerResurrection.PlayerBirth();  //讓玩家生成
				PlayerResurrection.ReDelete = false;  //重新開始 刪除
				Level_1.MissionTime = 0;
                Level_1.UiOpen = false;
                Level_1.LevelB_ = 1;
				PlayerView.Stop = true;  //UI隱藏
				AudioManager.AudioStop = false;
                if (Settings.GameLevel >= 2)
                {
					Save_Across_Scene.heroLife.closeDamageEffects(); //關閉受傷特效				
					Save_Across_Scene.Shooting.closeFireEffects(); //關閉攻擊特效
				}
    //            if (Settings.GameLevel == 1)
    //            {
				//	Settings.GameLevel = 2;
				//}

				//SceneManager.SetActiveScene(SceneManager.GetSceneByName(Globe.nextSceneName));
			}
		}
	}
}
