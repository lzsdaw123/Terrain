using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeWorkbench : MonoBehaviour
{
    public GameObject ObjectText;
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
    public GameObject Take;
    public GameObject AllObject;  //全物件
    public GameObject[] 升級UI;
    public UpgradeValue[] 武器欄位;  //(武器類型, 編號, 名稱, 圖片, 等級, 威力)
    public int DropdownType;
    public int FieldType;
    public int Save_PartObject;
    public int PartType;
    public static int 部位ID,零件ID;
    public string 部件名稱;
    public Text text;
    public static bool FirstWork;
    public float[] AddPower=new float[2];  //傷害加成
    public float[] AddRecoil = new float[2];  //後座力加成

    private void Awake()
    {
        AddPower = new float[] {1,1 };
        AddRecoil = new float[] {3.5f,0 };
        Save_PartObject = 0;
    }
    void Start()
    {
        Take = Save_Across_Scene.Take;
        ObjectText = Save_Across_Scene.ObjectText;
        Aim = Save_Across_Scene.Aim;
        GunCamera = Save_Across_Scene.Gun_Camera;
        Shop = GameObject.Find("Shop").GetComponent<Shop>();
        //UpgradeMenu = GameObject.Find("UpgradeMenu");
        UpgradeMenu.SetActive(false);
        UpCamTransform = GunCamera.gameObject.transform;
        FieldOfView = UpgradeCamera.GetComponent<Camera>().fieldOfView ;
        GunCamTransform = GunCamera.gameObject.transform;
        部位ID =零件ID = 0;
        部件名稱 = "不使用";
        time = -1;
        FirstWork = false;
        武器欄位[0].Power = AddPower[0] * AddPower[1];  //傷害 = 基礎 *槍口 * 彈匣
        武器欄位[0].Recoil = AddRecoil[0] + AddRecoil[1];  //後座力 = 基礎 *槍口 + 彈匣
        Shooting.UseWork(武器欄位);
    }

    void Update()
    {
        if (CamMove)
        {
            if (Move)  //拉近
            {
                if (time >= 0.9f || UpCamTransform.position == targetTransform[0].position)
                {
                    AllObject.SetActive(true);
                    CamMove = false;
                    UpgradeMenu.SetActive(true);
                    ////print("0");
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
            else  //拉遠
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
                }
                else if (time >= 0)
                {
                    FieldOfView -=12* Time.deltaTime;
                    time += Time.deltaTime;
                }
                UpCamTransform.localPosition = Vector3.SmoothDamp(UpCamTransform.localPosition, tagTranPos, ref currentVelocity, smoothTime, maxSpeed);
                UpCamTransform.rotation = Quaternion.Slerp(UpCamTransform.rotation, tagTranQu, 5f * Time.deltaTime);
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

        if (UpgradeMenu.activeSelf)
        {
            Shooting.UseWork(武器欄位);
        }
        if (Input.GetKeyDown(KeyCode.E))  //按 [E] 離開
        {
            if (Move && UpgradeMenu.activeSelf)
            {
                Exit();
            }
        }

        for (int i = 0; i < 武器欄位.Length; i++)  //武器欄位
        {
            for (int n = 0; n < 武器欄位[i].部位.Length; n++) //武器
            {
                for (int m = 0; m < 武器欄位[i].部位[n].Part.Length; m++)  //武器部位
                {
                    if (武器欄位[i].部位[n].Part[m] != null)
                    {
                        武器欄位[i].部位[n].ID[m] = m;
                        武器欄位[i].部位[n].PartName[m] = 武器欄位[i].部位[n].Part[m].name;
                    }
                }
            }
        }
        switch (部位ID)  //步槍
        {
            case 0:  //槍口
                switch (零件ID)
                {
                    case 0:
                        text.text = "[" + 部件名稱 + "]\n換取部件來獲得不同效果";
                        break;
                    case 1:
                        text.text = "[" + 部件名稱 + "]\n能降低開火時的火光";
                        break;
                    case 2:
                        text.text = "[" + 部件名稱 + "]\n能降低射擊後的後座力";
                        break;
                }
                break;
            case 1:  //彈匣
                switch (零件ID)
                {
                    case 0:
                        text.text = "[" + 部件名稱 + "]\n步槍用制式彈匣，30發裝彈量\n一般的制式彈匣";
                        break;
                    case 1:
                        text.text = "[" + 部件名稱 + "]\n步槍用擴充彈匣，70發裝彈量\n提高裝填量，稍微增加後座力";
                        break;
                    case 2:
                        text.text = "[" + 部件名稱 + "]\n步槍用穿甲彈匣，20發裝彈量\n有較強的穿透效果，後座力較大";
                        break;
                }
                break;
        }
    }
    public void UseType(int Type)  //下拉式選單
    {
        DropdownType = Type;
    }
    public void UsePartType(float Type)  //(0.0) 1部件 0.1武器欄位
    {
        PartType = (int)Type;  //部件類型
        FieldType = (int)(Type - PartType) * 10;  //武器欄位類型
    }
    public void UseDropdown(Dropdown dropdown)  //下拉式選單
    {
        switch (DropdownType)
        {
            case 0:  //換武器
                for (int i = 0; i < 武器欄位.Length; i++)
                {
                    武器欄位[i].Object.SetActive(false);
                    for (int n = 0; n < 武器欄位[i].部位.Length; n++)
                    {
                        武器欄位[i].部位[n].PartObject[1].SetActive(false);  //關閉全部武器
                    }
                }
                if (!武器欄位[dropdown.value].Object.activeSelf)
                {
                    武器欄位[dropdown.value].Object.SetActive(true);
                    for(int n=0; n< 武器欄位[dropdown.value].部位.Length; n++) 
                    {
                        武器欄位[dropdown.value].部位[n].PartObject[1].SetActive(true);   //打開當前武器
                    }
                }
                Save_PartObject = dropdown.value;
                break;
            case 1:  //換部件
                for (int i = 0; i < 武器欄位[FieldType].部位[PartType].Part.Length; i++)
                {
                    武器欄位[FieldType].部位[PartType].Part[i].SetActive(false);  //關閉全部零件
                }
                if (!武器欄位[FieldType].部位[PartType].Part[dropdown.value].activeSelf)
                {
                    switch (PartType)
                    {
                        case 0:  //槍口
                            武器欄位[FieldType].部位[PartType].Part[dropdown.value].SetActive(true);   //打開當前零件
                            AddPower[0] = 武器欄位[FieldType].部位[PartType].Power[dropdown.value];  //傷害加成
                            AddRecoil[0] = 武器欄位[FieldType].部位[PartType].Recoil[dropdown.value];  ///後座力加成
                            break;
                        case 1:  //彈匣
                            武器欄位[FieldType].部位[PartType].Part[dropdown.value].SetActive(true);   //打開當前零件
                            AddPower[1] = 武器欄位[FieldType].部位[PartType].Power[dropdown.value];  //傷害加成
                            AddRecoil[1] = 武器欄位[FieldType].部位[PartType].Recoil[dropdown.value];  ///後座力加成
                            武器欄位[FieldType].Ammo = 武器欄位[FieldType].部位[PartType].Ammo[dropdown.value];  //武器彈匣容量
                            break;
                    }
                    武器欄位[FieldType].Power = AddPower[0] * AddPower[1];  //傷害 = 基礎 *槍口 * 彈匣
                    武器欄位[FieldType].Recoil = AddRecoil[0] + AddRecoil[1];  //後座力 = 基礎 *槍口 + 彈匣
                    部位ID = PartType;
                    零件ID = 武器欄位[FieldType].部位[PartType].ID[dropdown.value];
                    部件名稱 = 武器欄位[FieldType].部位[PartType].PartName[dropdown.value];
                    play.GetComponent<Shooting>().enabled = true;
                    Shooting.換部件 = true;
                    //print(" "+Shooting.換部件);
                }
                break;
        }   
    }
    public void UseButton()  //使用按鈕
    {
    }

    public void Exit( )  //離開
    {
        UpgradeMenu.SetActive(false);
        AllObject.SetActive(false);  //關閉全部升級部件
        time = 0;
        CamMove = true;
        Move = false;
        for (int i = 0; i < 武器欄位.Length; i++)
        {
            武器欄位[i].Object.SetActive(false);
        }
        Shooting.UseWork(武器欄位);
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        ObjectText.GetComponent<Text>().text = "使用工作臺";
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            //Shop.OpenUI();
            tagTranPos = GunCamTransform.localPosition;
            tagTranQu = Quaternion.Euler(GunCamTransform.eulerAngles);
            FieldOfView = 55;
            play = Save_Across_Scene.Play;
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
            GunCamera.gameObject.GetComponent<MouseLook>().enabled = false;
            CamMove = true;
            Move = true;
            time = 0;
            武器欄位[Save_PartObject].Object.SetActive(true);
            升級UI[0].SetActive(true);
            if (!FirstWork)
            {
                FirstWork = true;
                if (PlayerView.missionLevel == 0)
                {
                    DialogueEditor.StartConversation(0, 4, 0, true, 0, true);  //開始對話
                }
            }
        }
    }
}
[Serializable]
public class UpgradeValue
{
    public String 類型;  //武器名稱
    public GameObject Object;  //武器
    public 部位[] 部位;  //武器部件
    public int Lvevl;  //等級
    public float Power =1;  //威力
    public float Recoil;  //後座力
    public int Ammo;  //彈藥量
    public float Price; //價格

    /// <summary>
    /// 各武器數值
    /// </summary>
    /// <param name="類型">武器類型</param>
    /// <param name="Object">武器</param>
    /// <param name="武器部位">武器部件</param>
    /// <param name="Lvevl">等級</param>
    /// <param name="Power">威力</param>
    /// <param name="Price">價格</param>
    /// <returns></returns>
    public UpgradeValue(String _類型, GameObject gameObject, 部位[] _部位, int lvevl, float power, float recoil, int ammo, float price)
    {
        類型 = _類型;
        Object = gameObject;
        部位 = _部位;
        Lvevl = lvevl;
        Power = power;
        Recoil = recoil;
        Ammo = ammo;
        Price = price;
    }
}
[Serializable]
public class 部位
{
    public String 類型;  //武器名稱
    public GameObject[] PartObject;  //武器部位
    public int[] ID; //編號
    public String[] PartName; //部件名稱
    public GameObject[] Part;  //武器部件
    public int[] Lvevl;  //等級
    public float[] Power;  //威力
    public float[] Recoil;  //後座力
    public int[] Ammo;  //彈藥量
    public float[] Price; //價格

    /// <summary>
    /// 各武器數值
    /// </summary>
    /// <param name="類型">武器類型</param>
    /// <param name="Object">武器</param>
    /// <param name="ID">編號</param>
    /// <param name="PartName">武器部件</param>
    /// <param name="Part">武器部件</param>
    /// <param name="Lvevl">等級</param>
    /// <param name="Power">威力</param>
    /// <param name="Price">價格</param>
    /// <returns></returns>
    public 部位(String _類型, GameObject[] partObject, int[] id, String[] partName, GameObject[] part, 
        int[] lvevl, float[] power, float[] recoil, int[] ammo, float[] price)
    {
        類型 = _類型;
        ID = id;
        PartName = partName;
        PartObject = partObject;
        Part = part;
        Lvevl = lvevl;
        Power = power;
        Recoil = recoil;
        Ammo = ammo;
        Price = price;
    }
}