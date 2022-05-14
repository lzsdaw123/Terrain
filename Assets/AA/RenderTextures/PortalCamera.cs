using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {
	public int Type;

	public Transform playerCamera; 
	public Transform portal;  //當前傳送門
	public Transform otherPortal;

    void Start()
    {
        if (playerCamera == null)
        {
			playerCamera = Save_Across_Scene.Gun_Camera.transform;
		}

	}
    void LateUpdate() {
		Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;

        if (Type == 0)
        {
			transform.position = portal.position + playerOffsetFromPortal;
		}
		float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);

		Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
		Vector3 newCameraDirection = portalRotationalDifference * playerCamera.forward;
		transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
	}
}
