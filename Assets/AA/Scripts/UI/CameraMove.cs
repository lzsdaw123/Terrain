using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    public GameObject MianCamera;
    public Vector3 CameraPos;
    public Transform MC_1, MC_2;

    public GameObject UI;  //升級UI
    public GameObject[] targetUI;
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
    [SerializeField] GameObject Aim;
    public GameObject Take;
    public GameObject AllObject;  //全物件
    public GameObject BossObject;  //全物件
    public bool locking;
    public Animator ani;
    public float StartBossTime;
    public bool StartBossT;
    public Level_1 level_1;
    GameObject DeBugT;
    public int Type;

    private void Awake()
    {
        Take = GameObject.Find("Take").gameObject;
        Aim = GameObject.Find("Aim").gameObject;
        DeBugT = GameObject.Find("DeBugT").gameObject;  //開發模式文字
    }
    public void StartBoos1()
    {
        StartBossTime = 0;
    }
    void Start()
    {
        if (BossObject != null) BossObject.SetActive(false);
        StartBossTime = -1;
        UI.SetActive(true);
        UpCamTransform = GunCamera.gameObject.transform;
        FieldOfView = UpgradeCamera.GetComponent<Camera>().fieldOfView;
        GunCamTransform = GunCamera.gameObject.transform;
        StartBossT = true;
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
    public void StartBoss1()
    {
        if (StartBossT)
        {
            StartBossT = false;
            Enter();
        }
    }

    void Update()
    {
        if (StartBossTime >= 0)
        {
            StartBossTime += Time.deltaTime;
            if (StartBossTime >= 2)
            {
                StartBossTime = -1;
                Level_1.StopAttack = false;
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
                if (time >= 0.9f || UpCamTransform.position == targetTransform[0].position)
                {
                    if(AllObject!=null) AllObject.SetActive(true);
                    if (BossObject != null) BossObject.SetActive(true);
                    CamMove = false;
                    FieldOfView = 60;
                    //Cursor.lockState = CursorLockMode.None; //游標無狀態模式
                    ani.SetBool("locking", true);
                }
                else if (time >= 0)
                {
                    FieldOfView += 12 * Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.position = Vector3.SmoothDamp(UpCamTransform.position, targetTransform[0].position, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, targetTransform[0].rotation, 6f * Time.smoothDeltaTime);
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
                    GunCamera.gameObject.GetComponent<QH_interactive>().enabled = true;
                    GunCamera.gameObject.GetComponent<QH_interactive>().ObjectText.SetActive(true);
                    Aim.SetActive(true);
                    Take.SetActive(true);
                    GunCamera.gameObject.GetComponent<MouseLook>().enabled = true;
                    UI.SetActive(true);
                    targetUI[0].GetComponent<Image>().enabled = true;
                    targetUI[1].GetComponent<Text>().enabled = true;
                }
                else if (time >= 0)
                {
                    FieldOfView -= 12 * Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.localPosition = Vector3.SmoothDamp(UpCamTransform.localPosition, tagTranPos, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, tagTranQu, 16f * Time.smoothDeltaTime);
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
        //Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
    }
    public void Enter()
    {
        UI.SetActive(false);
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
