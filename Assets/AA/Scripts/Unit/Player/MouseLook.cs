using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MouseLook : MonoBehaviour
{
    public float mouseSpeed = 100f;
    public float smoothSpeed;   //滑鼠靈敏度
    public Transform playerBody, m_transform,Gun;  //相機
    public float rotationX, rotationY = 0f;
    float camY = 2.865f;
    float camZ = 0.089f;
    public Vector3 m_camRot;
    public Transform CameraPos;
    public Camera GunCamera;
    public GameObject GunObject;  //槍枝物件
    public float GRx,GRy;  //槍支攝影機的Rotation
    Vector3 oldPos;  //上一幀
    Vector3 newPos; //當前幀
    float chaX,chaY;  //兩幀的螢幕座標差值結果

    public GameObject UI;  //邊框UI
    RectTransform oriTransform;   // UI位置
    Vector3 currentVelocity = Vector3.zero;     // 當前速度，這個值由你每次呼叫這個函式時被修改
    float UD_Speed = 50;    // 介面上下位移速度
    float LR_Speed = 40;    // 介面左右位移速度
    float minSpeed = 620;    // 介面回復速度
    float smoothTime = 0.35f;      // 達到目標大約花費的時間。 一個較小的值將更快達到目標。
    Vector3 oriRTPos,newRTPos;
    static bool shake;
    public float mouseX, mouseY;

    public float smooth = 3;          // 相機移動的平穩程度

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式

        m_transform = this.transform;        // 設置攝像機初始位置

        oldPos = CameraPos.rotation.eulerAngles; //上一幀攝影機的歐拉角

        oriTransform = UI.GetComponent<RectTransform>();
        newRTPos=oriRTPos = oriTransform.transform.position;
    }
    void Update()
    {
        if (shake)
        {
            shake = false;
            //newRTPos = UI.GetComponent<RectTransform>().position;  //UI晃動
            //float[] FRxMin = new float[] { 20, 14 * 2, 16 * 2 };  //最小垂直晃動 x
            //float[] FRxMax = new float[] { 40, 26 * 2, 30 * 2 };  //最大垂直晃動 x
            //float rangeY = Random.Range(-10f, 10f);  //射擊水平晃動範圍
            //float rangeX = Random.Range(FRxMin[0], FRxMax[0]);  //射擊垂直晃動範圍
            //newRTPos.x += rangeX;
            //newRTPos.y += rangeY;
            //oriTransform.transform.position = newRTPos;

            //rotationX -= Random.Range(0.4f, 1f);  //開火後鏡頭垂直晃動範圍
            //rotationY = Random.Range(-8f, 8f) * Time.deltaTime;  //開火後鏡頭水平晃動範圍
        }

        //}
        //void LateUpdate()
        //{
        // 獲得鼠標當前位置的X和Y                                
        mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.smoothDeltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.smoothDeltaTime;
        newPos = CameraPos.rotation.eulerAngles; //當前幀攝影機的歐拉角

        if (newPos == oldPos)  //攝影機是否轉動
        {
            if (GRy > 0.5f) //槍枝向右歸位
            {
                GRy -= 20 * Time.smoothDeltaTime;
                if (GRy < 0)
                {
                    GRy = 0;
                }
            }
            else if (GRy < -0.5f)  //槍枝向左歸位
            {
                GRy += 20 * Time.smoothDeltaTime;
                if (GRy > 0)
                {
                    GRy = 0;
                }
            }
        }
        else
        {
            chaY = newPos.y - oldPos.y;
            chaX = newPos.x - oldPos.x;

            if (chaY > 1.5f)  //鏡頭向右移
            {
                newRTPos.x -= LR_Speed * Time.smoothDeltaTime;
                GRy += 20 * Time.smoothDeltaTime;
                if (GRy >= 4)
                {
                    GRy = 4;
                }
            }
            else if (chaY < -1.5f)  //鏡頭向左移
            {
                newRTPos.x += LR_Speed * Time.smoothDeltaTime;
                GRy -= 20 * Time.smoothDeltaTime;
                if (GRy <= -4)
                {
                    GRy = -4;
                }
            }
            if (chaX > 0.5f)  //鏡頭向下移
            {
                newRTPos.y += UD_Speed * Time.smoothDeltaTime;
            }
            else if (chaX < -0.5f)  //鏡頭向上移
            {
                newRTPos.y -= UD_Speed * Time.smoothDeltaTime;
            }
        }
        oldPos = newPos;
        if (newRTPos != oriRTPos)  //當UI位移
        {
            newRTPos = Vector3.SmoothDamp(newRTPos, oriRTPos, ref currentVelocity, smoothTime, minSpeed);
        }
        else
        {
            newRTPos = oriRTPos;
        }
        oriTransform.transform.position = newRTPos;
        ////GunCamera.transform.localRotation = Quaternion.Euler(0, 0, GRy);  //武器左右晃
        GunObject.transform.localRotation = Quaternion.Euler(0, 0, -GRy);  //武器左右晃

        smoothSpeed = Settings.smoothSpeed;
        //print(smoothSpeed);
        rotationX -= mouseY * smoothSpeed * Time.smoothDeltaTime;  //滑鼠控制鏡頭上下
        rotationX = Mathf.Clamp(rotationX, -85f, 80f);

        Gun.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);  //相機位移
        playerBody.Rotate(Vector3.up * mouseX * smoothSpeed * Time.smoothDeltaTime);  //滑鼠控制鏡頭左右
        rotationY = 0;  //相機Y軸歸零
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
    public static void Shaking()
    {
        shake = true;
    }
}
