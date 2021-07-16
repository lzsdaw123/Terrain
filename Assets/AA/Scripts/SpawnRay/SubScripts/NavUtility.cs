using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavUtility
{

	// 亂數取得圓形範圍內的點
	public Vector3 RandomCirclePos(Vector3 position, float radius){
		Quaternion rotation = Quaternion.Euler (0, Random.value * 360f, 0);
		return rotation * new Vector3 (0, 0, Random.value * radius) + position;
	}

	// 亂數取得矩形範圍內的點
	public Vector3 RandomRectanglePos(Vector3 position, float width, float depth){
		float offset_x = -width / 2f;
		float offset_z = -depth / 2f;

		return new Vector3 (Random.value * width + offset_x, 0, Random.value * depth + offset_z) + position;
	}

	// 判斷是否有打到可走區域
	public bool TryHitNav(Vector3 position, out RaycastHit hitInfo){

		hitInfo = new RaycastHit ();
		Vector3 rayPoint = position + new Vector3 (0, 1000, 0);
		Ray ray = new Ray (rayPoint, -Vector3.up);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit )) {
			#if UNITY_EDITOR
			Debug.DrawRay(ray.origin, hit.point-ray.origin, Color.red, 3);
			#endif

			NavMeshHit navHit;
			if (NavMesh.SamplePosition (hit.point, out navHit, 1, NavMesh.AllAreas)) {
				hitInfo = hit;
				return true;
			} else
				return false;
		} else
			return false;
	}

	public bool TryHitNav(Vector3 position, out RaycastHit hitInfo, float maxDistance, LayerMask layerMask){

		hitInfo = new RaycastHit ();
		Vector3 rayPoint = position + new Vector3 (0, 1000, 0);
		Ray ray = new Ray (rayPoint, -Vector3.up);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, maxDistance, layerMask )) {
			#if UNITY_EDITOR
			Debug.DrawRay(ray.origin, hit.point-ray.origin, Color.red, 3);
			#endif

			NavMeshHit navHit;
			if (NavMesh.SamplePosition (hit.point, out navHit, 1, NavMesh.AllAreas)) {
				hitInfo = hit;
				return true;
			} else
				return false;
		} else
			return false;
	}
}
