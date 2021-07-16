using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSpeed = 100f;
    public Transform playerBody, m_transform,Gun;
    public float rotationX = 0f;
    float camY = 2.865f;
    float camZ = 0.089f;
    public Vector3 m_camRot;

    public float mouseX, mouseY;

    public float smooth = 3;                // 相機移動的平穩程度


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式

        m_transform = this.transform;
        // 設置攝像機初始位置
    }

    void LateUpdate()
    {
        // 獲得鼠標當前位置的X和Y                                
        mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -85f, 80f);

		Gun.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
		playerBody.Rotate(Vector3.up * mouseX);

        Vector3 playerBodyP = new Vector3(Gun.position.x, Gun.position.y, Gun.position.z);

        // 設置攝像機的旋轉方向與主角一致
        m_transform.rotation = Gun.rotation; //rotation為物體在世界坐標中的旋轉角度，用Quaternion賦值
        m_transform.position = playerBodyP; //rotation為物體在世界坐標中的旋轉角度，用Quaternion賦值

        //m_camRot.x = rotationX;
        //m_camRot.y = mouseY;
        //m_transform.transform.eulerAngles = m_camRot; //通過改變XYZ軸的旋轉改變歐拉角

        //// 使主角的面向方向與攝像機一致
        //Vector3 camrot = playerBody.transform.eulerAngles;
        //camrot.x = 0; camrot.z = 0;
        //playerBody.eulerAngles = camrot;

   
        
    }
}
