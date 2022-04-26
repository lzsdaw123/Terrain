using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    public GameObject CanvasUI;  //全部UI
    public GameObject[] targetUI;
    public GameObject play; //玩家
    public GameObject Hit_Play;  //擊中玩家彈孔
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
    [SerializeField] GameObject Aim;
    public GameObject Take;
    public GameObject AllObject;  //全物件
    public GameObject BossObject;  //Boss物件
    public bool locking;
    public Animator ani;
    public float CM_EndTime;
    public bool Enable_Camera;
    public Level_1 level_1;
    GameObject DeBugT;
    public int CM_Type;  //哪台相機運鏡
    public float MoveSpeed;  //運鏡速度
    public float DelayTime;  //延遲運行時間
    public bool Duble;  //延遲運行時間

    private void Awake()
    {
        Take = GameObject.Find("Take").gameObject;
        Aim = GameObject.Find("Aim").gameObject;
        DeBugT = GameObject.Find("DeBugT").gameObject;  //開發模式文字
    }
    public void CameraMoveEnd(float Time)  //運鏡結束時間
    {
        CM_EndTime = Time;
    }
    void Start()
    {
        if (BossObject != null) BossObject.SetActive(false);
        CM_EndTime = -1;
        CanvasUI.SetActive(true);
        UpCamTransform = GunCamera.gameObject.transform;
        FieldOfView = UpgradeCamera.GetComponent<Camera>().fieldOfView;
        GunCamTransform = GunCamera.gameObject.transform;
        Enable_Camera = true;
        DelayTime = -1;
        DontDestroyOnLoad(gameObject);  //切換場景時保留
    }
    private void lockingMove(int Lock)
    {
        switch (Lock)
        {
            case 0:
                locking = false;
                break;
            case 1:
                locking = true;
                break;
        }
    }
    public void CameraMovement(int Type,float moveSpeed, float delayTime, bool duble)  //呼叫相機 (哪一台, 延遲運行時間, 二次位移)
    {
        CM_Type = Type;
        MoveSpeed = moveSpeed;
        DelayTime = delayTime;
        Duble = duble;
    }

    void Update()
    {
        if (DelayTime >= 0)
        {
            DelayTime -= Time.deltaTime;
            if(DelayTime<=0 && DelayTime > -1)
            {
                if (Enable_Camera)
                {
                    DelayTime = -1;
                    Enable_Camera = false;
                    Enter();
                }
            }
        }
        if (CM_EndTime > 0)
        {
            CM_EndTime -= Time.deltaTime;
            if (CM_EndTime <= 0)
            {
                CM_EndTime = -1;
                switch (CM_Type)
                {
                    case 0:
                        Level_1.StopAttack = false;
                        break;
                }
                Exit();
            }
        }
        if (locking)
        {
            //Vector3 targetDir = BossObject.transform.position - GunCamTransform.position;
            //Quaternion rotate = Quaternion.LookRotation(targetDir);
            //GunCamTransform.localRotation = Quaternion.Slerp(GunCamTransform.localRotation, rotate, 1f * Time.deltaTime);
            //UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, targetTransform[1].rotation, 0.5f * Time.deltaTime);
            UpCamTransform.position = Vector3.SmoothDamp(UpCamTransform.position, targetTransform[1].position, ref currentVelocity, smoothTime, maxSpeed);
            UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, targetTransform[1].rotation, 0.5f * Time.smoothDeltaTime);
        }
        if (Input.GetKey(KeyCode.O))
        {
            Enter();
        }
        if (Input.GetKey(KeyCode.P))
        {
            Exit();
        }
        if (CamMove)
        {
            if (Move)  //拉近 綁定鏡頭
            {
                if (time >= 0.9f || UpCamTransform.position == targetTransform[CM_Type].position)
                {
                    if(AllObject!=null) AllObject.SetActive(true);
                    switch (CM_Type)
                    {
                        case 0:
                            if (BossObject != null) BossObject.SetActive(true);  //啟動水晶boss
                            break;
                    }
                    CamMove = false;
                    FieldOfView = 60;
                    //Cursor.lockState = CursorLockMode.None; //游標無狀態模式
                    switch (Duble)
                    {
                        case true:
                            ani.SetBool("locking", true);
                            break;
                    }
                }
                else if (time >= 0)
                {
                    FieldOfView += 12 * Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.position = Vector3.SmoothDamp(UpCamTransform.position, targetTransform[CM_Type].position, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, targetTransform[CM_Type].rotation, MoveSpeed * Time.smoothDeltaTime);
            }
            else  //拉遠 解除鏡頭
            {
                if (time >= 0.9f || UpCamTransform.position == tagTranPos)
                {
                    CamMove = false;
                    FieldOfView = 55;
                    play.GetComponent<PlayerMove>().enabled = true;
                    if (Shooting.FirstWeapon[0] == true)
                    {
                        play.GetComponent<Shooting>().Weapon.SetBool("LayDown", false);
                    }
                    play.GetComponent<Shooting>().enabled = true;
                    Hit_Play.SetActive(true);
                    GunCamera.gameObject.GetComponent<QH_interactive>().enabled = true;
                    GunCamera.gameObject.GetComponent<QH_interactive>().ObjectText.SetActive(true);
                    Aim.SetActive(true);
                    Take.SetActive(true);
                    GunCamera.gameObject.GetComponent<MouseLook>().enabled = true;
                    CanvasUI.SetActive(true);
                    targetUI[0].GetComponent<Image>().enabled = true;
                    targetUI[1].GetComponent<Text>().enabled = true;
                }
                else if (time >= 0)
                {
                    FieldOfView -= 12 * Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.localPosition = Vector3.SmoothDamp(UpCamTransform.localPosition, tagTranPos, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, tagTranQu, MoveSpeed *3 * Time.smoothDeltaTime);
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
    }
    public void Exit()  //離開
    {
        if (AllObject != null) AllObject.SetActive(false);  //關閉全部升級部件
        time = 0;
        locking = false;
        CamMove = true;
        Move = false;
        Enable_Camera = true;
        //Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
    }
    public void Enter()
    {
        CanvasUI.SetActive(false);
        targetUI[0].GetComponent<Image>().enabled = false;
        targetUI[1].GetComponent<Text>().enabled = false;
        tagTranPos = GunCamTransform.localPosition;
        tagTranQu = Quaternion.Euler(GunCamTransform.eulerAngles);
        FieldOfView = 55;
        play = GameObject.Find("POPP").gameObject;
        if (Shooting.FirstWeapon[0] == true)
        {
            play.GetComponent<Shooting>().Weapon.SetTrigger("LayDownT");
            play.GetComponent<Shooting>().Weapon.SetBool("LayDown", true);
        }
        play.GetComponent<Shooting>().enabled = false;
        PlayerMove.Player_h = 0;
        PlayerMove.Player_v = 0;
        play.GetComponent<PlayerMove>().enabled = false;
        Hit_Play.SetActive(false);
        GunCamera.gameObject.GetComponent<QH_interactive>().enabled = false;
        GunCamera.gameObject.GetComponent<QH_interactive>().ObjectText.SetActive(false);
        Aim.SetActive(false);
        Take.SetActive(false);
        DeBugT.SetActive(false);
        GunCamera.gameObject.GetComponent<MouseLook>().enabled = false;
        CamMove = true;
        Move = true;
        time = 0;
        //DialogueEditor.StartConversation(0, 4, 0, true, 0);  //開始對話
    }
}
