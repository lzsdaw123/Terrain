﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class SpawnRay : MonoBehaviour {

	[SerializeField] bool DisplayField = true;
	public GameObject[] monsters = new GameObject[1];	// 可生的怪種類
	public int maxNumber = 5;							// 生怪上限
	[SerializeField] private float bornTimespanMin = 3f;	// 下次生怪時間間隔
	[SerializeField] private float bornTimespanMax = 10f;
	private float nextBornTime = 0f;
	private float timer = 0;							// 計數器
	public int counter{ get; set; }						// 目前場上怪物數量
	private int uid = 0;								// 怪物編號


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
		counter = 0;
		uid = 0;
	}

	void Update(){
		if (counter >= maxNumber)	// 若已達生怪上限，返回
			return;
		
		timer += Time.deltaTime;

		if (timer > nextBornTime) {	// 若已達生怪時間間隔

			// 計算生怪亂數位置
			Vector3 bornPos = transform.position;
			if (type == Type.Circle)
				bornPos = navUtility.RandomCirclePos (transform.position, bornRadius);
			else
				bornPos = navUtility.RandomRectanglePos (transform.position, bornWidth, bornDepth);

			RaycastHit hit;
			if (navUtility.TryHitNav (bornPos, out hit, Mathf.Infinity, rayHitLayer)) {	// 判斷位置是否可生怪
				
				int monsterNum = (int)(Random.value * monsters.Length);	// 亂數取得一隻怪
				GameObject monster = (GameObject)Instantiate (monsters [monsterNum], hit.point, Quaternion.identity);	// 生怪
				uid++;										// 編號加1
				counter++;

				if (!monster.GetComponent<SpawnRayReg> ())	// 怪物一定要有這個腳本
					monster.AddComponent<SpawnRayReg> ();
				
				monster.SendMessage ("Init", new MonterInfo(uid, this));
			}

			timer = 0;
			nextBornTime = Random.Range (bornTimespanMin, bornTimespanMax);	// 亂數取得下次生怪時間

		}
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


	public void UnReg(){
		if (counter >= maxNumber) { // 若怪物已滿，重新計時，以免立即生怪
			timer = 0;
		}
		counter--;
	}

}




public struct MonterInfo{
	public int uniqueID;	// 怪物編號
	public SpawnRay mother;

	public MonterInfo(int UniqueID, SpawnRay Mother){
		uniqueID = UniqueID;
        mother = Mother;
	}
}

