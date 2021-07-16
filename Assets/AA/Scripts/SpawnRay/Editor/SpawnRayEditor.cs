using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 處理動態選單
[CustomEditor(typeof(SpawnRay))]
public class SpawnRayEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
        SpawnRay spawn = (SpawnRay)this.target;
		switch (spawn.type)
		{
			case SpawnRay.Type.Circle:
                spawn.bornRadius = EditorGUILayout.FloatField("Born Radius", spawn.bornRadius);
                spawn.liveRadius = EditorGUILayout.FloatField("Live Radius", spawn.liveRadius);
				break;
			case SpawnRay.Type.Rectangle:
                spawn.bornWidth = EditorGUILayout.FloatField("Born Width", spawn.bornWidth);
                spawn.bornDepth = EditorGUILayout.FloatField("Born Depth", spawn.bornDepth);
                spawn.liveWidth = EditorGUILayout.FloatField("Live Width", spawn.liveWidth);
                spawn.liveDepth = EditorGUILayout.FloatField("Live Depth", spawn.liveDepth);
				break;
		}
		SceneView.RepaintAll();
	}

}