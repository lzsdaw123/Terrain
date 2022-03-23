using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWorkbench : MonoBehaviour
{
    public GameObject TextG;
    public int Type;
    public Shop Shop; 
    public GameObject UpgradeMenu;  //升級UI
    public GameObject play;
    public Camera GunCamera;  //玩家相機
    public Camera UpgradeCamera;  //升級相機
    public Transform GunCamTransform;  //玩家相機位置
    public Transform UpCamTransform;  //升級相機位置
    public Transform[] targetTransform;  //指定位置
    public bool CamMove;  //是否相機位移
    public bool Move;  //相機位移與歸位
    private Vector3 currentVelocity = Vector3.zero;     // 當前速度，這個值由你每次呼叫這個函式時被修改
    float maxSpeed = 40f;    // 選擇允許你限制的最大速度
    float smoothTime = 0.25f;      // 達到目標大約花費的時間。 一個較小的值將更快達到目標。
    public float time;  //位移時間
    public Vector3 tagTranPos;
    public Quaternion tagTranQu;
    float FieldOfView;  //相機視野
    GameObject Aim;
    GameObject Take;

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Shop = GameObject.Find("Shop").GetComponent<Shop>();
        UpgradeMenu = GameObject.Find("UpgradeMenu");
        UpgradeMenu.SetActive(false);
        UpCamTransform = GunCamera.gameObject.transform;
        FieldOfView = UpgradeCamera.GetComponent<Camera>().fieldOfView ;
        GunCamTransform = GunCamera.gameObject.transform;
        Aim = GameObject.Find("Aim").gameObject;
        Take = GameObject.Find("Take").gameObject;

        time = -1;
    }

    void Update()
    {
        if (CamMove)
        {
            if (Move)
            {
                if (time >= 0.9f || UpCamTransform.position == targetTransform[0].position)
                {
                    CamMove = false;
                    UpgradeMenu.SetActive(true);
                    FieldOfView = 60;
                    Cursor.lockState = CursorLockMode.None; //游標無狀態模式
                }
                else if (time >= 0)
                {
                    FieldOfView +=12* Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.position = Vector3.SmoothDamp(UpCamTransform.position, targetTransform[0].position, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, targetTransform[0].rotation, 5f * Time.deltaTime);
            }
            else
            {
                if (time >= 0.9f || UpCamTransform.position == tagTranPos)
                {
                    CamMove = false;
                    FieldOfView = 55;
                    play.GetComponent<PlayerMove>().enabled = true;
                    play.GetComponent<Shooting>().Weapon.SetBool("LayDown", false);
                    play.GetComponent<Shooting>().enabled = true;
                    GunCamera.gameObject.GetComponent<QH_interactive>().enabled = true;
                    GunCamera.gameObject.GetComponent<QH_interactive>().ObjectText.SetActive(true);
                    Aim.SetActive(true);
                    Take.SetActive(true);
                    GunCamera.gameObject.GetComponent<MouseLook>().enabled = true;

                }
                else if (time >= 0)
                {
                    FieldOfView -=12* Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.localPosition = Vector3.SmoothDamp(UpCamTransform.localPosition, tagTranPos, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, tagTranQu, 5f * Time.deltaTime);
            }      
        }
        if (FieldOfView <= 55)
        {
            FieldOfView = 55;
        }
        if (FieldOfView >= 60)
        {
            FieldOfView = 60;
        }
        GunCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
    }
    public void Exit( )
    {
        time = 0;
        CamMove = true;
        Move = false;
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        TextG.GetComponent<Text>().text = "按「E」使用工作臺";
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            //Shop.OpenUI();
            tagTranPos = GunCamTransform.localPosition;
            tagTranQu = Quaternion.Euler(GunCamTransform.eulerAngles);
            FieldOfView = 55;
            play = GameObject.Find("POPP").gameObject;
            play.GetComponent<PlayerMove>().enabled = false;
            play.GetComponent<Shooting>().Weapon.SetTrigger("LayDownT");
            play.GetComponent<Shooting>().Weapon.SetBool("LayDown", true);
            play.GetComponent<Shooting>().enabled = false;
            GunCamera.gameObject.GetComponent<QH_interactive>().enabled = false;
            GunCamera.gameObject.GetComponent<QH_interactive>().ObjectText.SetActive(false);
            Aim.SetActive(false);
            Take.SetActive(false);
            GunCamera.gameObject.GetComponent<MouseLook>().enabled = false;

            CamMove = true;
            Move = true;
            time = 0;
        }
    }
}
