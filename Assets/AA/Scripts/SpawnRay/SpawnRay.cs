using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
//using System;

public class SpawnRay : MonoBehaviour {

	[SerializeField] bool DisplayField = true;
	public GameObject[] monsters = new GameObject[1];	// 可生的怪種類
	public int[] maxNumber = new int[] {7, 5 };							// 生怪上限
	[SerializeField] private float bornTimespanMin = 3f;	// 下次生怪時間間隔
	[SerializeField] private float bornTimespanMax = 10f;
	private float[] nextBornTime = new float[] { 0, 0 };
	private float[] timer =new float[] {0, 0 };                         // 計數器
	public int[] counter { get; set; } = new int[] { 0, 0 };		// 目前場上怪物數量
    [SerializeField] int[] SF_counter;		// 目前場上怪物數量
	private int uid = 0;                                // 怪物編號

	[SerializeField] private LayerMask rayHitLayer = -1;	// 射線判斷的圖層，-1表示全部圖層
	public enum Type  {Circle, Rectangle};	// 圓形或矩形生怪區域
	public Type type = Type.Circle;			// 預設為圓形

	// 圓形生怪磚
	[HideInInspector] public float bornRadius = 5f;		// 生怪範圍
	[HideInInspector] public float liveRadius = 15f;	// 怪物移動範圍
	//	public float bornRadius{ get { return _bornRadius; } set { _bornRadius = value; } }
	//	public float moveRadius{ get { return _moveRadius; } set { _moveRadius = value; } }

	// 方形生怪磚
	[HideInInspector] public float bornWidth = 10f;
	[HideInInspector] public float bornDepth = 10f;
	[HideInInspector] public float liveWidth = 30f;
	[HideInInspector] public float liveDepth = 30f;

    NavUtility navUtility = new NavUtility();

	public ObjectPool _ObjectPool;
	public bool StartBorn;  //開始生成
	public int EnemyWave;  //敵人波數
	public EnemyWaveNum[] EnemyNum;   //敵人每波數量
	bool[] StartBool=new bool[2];
	public float BornTime;

	void OnDrawGizmos(){
		if (DisplayField) {
			if (type == Type.Circle) {
				Gizmos.color = Color.yellow;
				//Gizmos.DrawWireSphere (transform.position, bornRadius);	// 繪製生怪範圍
				GizmosExtension.DrawCylinder(transform.position, 5f, bornRadius);
				Gizmos.color = Color.cyan;
				//Gizmos.DrawWireSphere (transform.position, liveRadius);	// 繪製移動範圍
				GizmosExtension.DrawCylinder(transform.position, 5f, liveRadius);
			} else {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireCube (transform.position, new Vector3 (bornWidth, 5, bornDepth));
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube (transform.position, new Vector3 (liveWidth, 5, liveDepth));
			}
		}
	}

	void Start(){
		counter = new int[] { 0, 0 };
		uid = 0;
		StartBorn = true;
		EnemyNum = new EnemyWaveNum[3];
        EnemyNum[0] = new EnemyWaveNum(4, 2);
        EnemyNum[1] = new EnemyWaveNum(6, 4);
        EnemyNum[2] = new EnemyWaveNum(6, 4);
        //EnemyNum[3] = new EnemyWaveNum(8, 6);
        //EnemyNum[4] = new EnemyWaveNum(10, 6);
        //EnemyNum[0] = new EnemyWaveNum(0, 1);
        //EnemyNum[1] = new EnemyWaveNum(0, 1);
        //EnemyNum[2] = new EnemyWaveNum(0, 1);
    }
	public void EnemyWaveNum(int enemyWave)
    {
		EnemyWave = enemyWave;
	}
	void Update()
	{
		maxNumber[0] = EnemyNum[EnemyWave].EnemyNumA;
		maxNumber[1] = EnemyNum[EnemyWave].EnemyNumB;
		SF_counter = counter;

		if (StartBool[0] || StartBool[1] && StartBorn)  //復活生成
        {
			if (BornTime >= 20)  //停止復活生成  
			{
				BornTime = -1;
				StartBorn = false;
				StartBool[0] = false;
                switch (Level_1.LevelA_)
                {
					//case 4:
					//	Level_1.stageTime = 25;  //開始進階冷卻
					//	break;
					case 5:
						Level_1.LevelA_ = 6;
						break;
					case 7:
						Level_1.LevelA_ = 8;
						break;
				}
			}
			else if (BornTime >= 0)
			{
				BornTime += Time.deltaTime;
			}
		}


		if (!StartBorn) return;
		if (counter[0] < EnemyNum[EnemyWave].EnemyNumA)    // 若已達生怪上限，返回
		{
			timer[0] += Time.deltaTime;
			StartBool[0] = false;
			if (timer[0] > nextBornTime[0])
			{   // 若已達生怪時間間隔

				// 計算生怪亂數位置
				Vector3 bornPos = transform.position;
				if (type == Type.Circle)
					bornPos = navUtility.RandomCirclePos(transform.position, bornRadius);
				else
					bornPos = navUtility.RandomRectanglePos(transform.position, bornWidth, bornDepth);

				RaycastHit hit;
				if (navUtility.TryHitNav(bornPos, out hit, Mathf.Infinity, rayHitLayer))
				{ // 判斷位置是否可生怪

					//int monsterNum = (int)(Random.value * monsters.Length);	// 亂數取得一隻怪
					//GameObject monster = (GameObject)Instantiate (monsters [monsterNum], hit.point, Quaternion.identity);   // 生怪
					_ObjectPool.ReUseMonster01(hit.point, Quaternion.identity);  //呼叫物件池
					uid++;                                      // 編號加1
					counter[0]++;

					//if (!monster.GetComponent<SpawnRayReg> ())	// 怪物一定要有這個腳本
					//	monster.AddComponent<SpawnRayReg> ();

					//monster.SendMessage ("Init", new MonterInfo(uid, this));
				}

				timer[0] = 0;
				nextBornTime[0] = Random.Range(bornTimespanMin, bornTimespanMax);   // 亂數取得下次生怪時間

			}
		}else StartBool[0] = true;

		if (counter[1] < EnemyNum[EnemyWave].EnemyNumB)    // 若已達生怪上限，返回
		{
			timer[1] += Time.deltaTime;
			StartBool[1] = false;
			if (timer[1] > nextBornTime[1])
			{   // 若已達生怪時間間隔

				// 計算生怪亂數位置
				Vector3 bornPos = transform.position;
				if (type == Type.Circle)
					bornPos = navUtility.RandomCirclePos(transform.position, bornRadius);
				else
					bornPos = navUtility.RandomRectanglePos(transform.position, bornWidth, bornDepth);

				RaycastHit hit;
				if (navUtility.TryHitNav(bornPos, out hit, Mathf.Infinity, rayHitLayer))
				{ // 判斷位置是否可生怪

					//int monsterNum = (int)(Random.value * monsters.Length);	// 亂數取得一隻怪
					//GameObject monster = (GameObject)Instantiate (monsters [monsterNum], hit.point, Quaternion.identity);   // 生怪
					_ObjectPool.ReUseMonster02(hit.point, Quaternion.identity);  //呼叫物件池
					uid++;                                      // 編號加1
					counter[1]++;

					//if (!monster.GetComponent<SpawnRayReg> ())	// 怪物一定要有這個腳本
					//	monster.AddComponent<SpawnRayReg> ();

					//monster.SendMessage ("Init", new MonterInfo(uid, this));
				}

				timer[1] = 0;
				nextBornTime[1] = Random.Range(bornTimespanMin, bornTimespanMax);   // 亂數取得下次生怪時間

			}
		}else StartBool[1] = true;

	}


	// 試著取一個目標點供角色遊走
	public bool TryGetRandomLivePoint(out RaycastHit hitInfo){
		// 計算目標點亂數位置
		Vector3 targetPos = transform.position;
		if (type == Type.Circle)
			targetPos = navUtility.RandomCirclePos (transform.position, liveRadius);
		else
			targetPos = navUtility.RandomRectanglePos (transform.position, liveWidth, liveDepth);

		hitInfo = new RaycastHit ();
		RaycastHit hit;
		if (navUtility.TryHitNav (targetPos, out hit, Mathf.Infinity, rayHitLayer)) {	// 判斷位置是否可走
			hitInfo = hit;
			return true;
		} else
			return false;
	}

	// 判斷是否在活動範圍內，範圍內傳回true，範圍外傳回false
	public bool InLiveZone(Vector3 position){
		if (type == Type.Circle) {
			if (Vector3.Distance (position, transform.position) < liveRadius)
				return true;
			else
				return false;
		} else {
			Vector3 dis = position - transform.position;
			float wDistance = Mathf.Abs (dis.x);
			float dDistance = Mathf.Abs (dis.z);
			if (wDistance < liveWidth * 0.5f || dDistance < liveDepth * 0.5f)
				return true;
			else
				return false;
		}
	}


	public void UnReg(int Type){	
		if (counter[Type] >= maxNumber[Type]) { // 若怪物已滿，重新計時，以免立即生怪
			timer[Type] = 0;
		}
		counter[Type]--;
	}

}

public struct MonterInfo{
	public int uniqueID;	// 怪物編號
	public SpawnRay mother;
	public int MpnsterType;

	public MonterInfo(int UniqueID, SpawnRay Mother, int Type){
		uniqueID = UniqueID;
        mother = Mother;
		MpnsterType = Type;
	}
}

