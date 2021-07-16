﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRayReg : MonoBehaviour {

	private int uid = 0;
	public int actorID{ get{return uid;} }	// 怪物編號
	[HideInInspector] public SpawnRay mother;

	public void Init(MonterInfo monsterInfo){
		mother = monsterInfo.mother;
		uid = monsterInfo.uniqueID;
	}

	public void OnDestroy(){
		if (mother)
			mother.UnReg();		// 向母體移除數量
	}
}
