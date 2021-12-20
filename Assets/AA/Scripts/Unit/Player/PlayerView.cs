using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 將指令碼掛在攝像機觀察的物體上  物體必須帶有Render
/// </summary>
public class PlayerView : MonoBehaviour
{
    public DialogueEditor DialogueEditor;
    bool isRendering;
    float curtTime = 0f;
    float lastTime = 0f;
    public Camera Camera;
    public Transform camTransform;
    public  GameObject[] MissionTaget;  //任務目標物件
    public static int missionLevel;  //任務階段
    [SerializeField] private int st_missionLevel;  //任務階段
    public float Rdot;
    public float Fdot;
    public Image targetUI;  //任務目標UI
    public Text text;
    public float UI_x;
    public float UI_y;
    public Vector3 Camdir;
    public Vector2 SP;
    public float dot;
    bool sp_Dot;
    bool sp_Dot_D=true;
    [SerializeField] float distance;  //距離
    public static float pu_distance;
    [SerializeField] int d;  //換算距離
    Color UIcolor;
    Vector3 oldPos;
    public static bool MissionEnd;

    void OnWillRenderObject()
    {
        curtTime = Time.time;
    }
    public bool IsInView(Vector3 worldPos)
    {
        camTransform = Camera.transform;  //相機座標
        Vector2 viewPos = Camera.WorldToViewportPoint(worldPos);  //世界座標到視口座標
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float c_dot = Vector3.Dot(camTransform.forward, dir);     //判斷物體是否在相機前面


        if (c_dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }
    public static void TagetChange()  //改變目標
    {
        if (missionLevel == 10)
        {
            Level_1.MissionEnd = true;
            MissionEnd = true;
        }
        else
        {
            missionLevel++;
        }
        Level_1.MissionTime = 0;
        Level_1.UiOpen = true;
        Level_1.StartDialogue = true;
    }
    void Start()
    {
        missionLevel = 0;
    }
    void Update()
    {
        st_missionLevel = missionLevel;
        if (MissionEnd)  //任務目標結束
        {
            targetUI.color = new Color(1, 1, 1, 0);
            text.color = new Color(1, 1, 1, 0);
            return;
        }
        //Vector2 vec2 = Camera.WorldToScreenPoint(this.gameObject.transform.position);  //世界座標到螢幕座標
        camTransform = Camera.transform;  //相機座標
        distance = (camTransform.position - MissionTaget[missionLevel].transform.position).magnitude / 3.5f;

        pu_distance = distance;
        d = (int)distance;
        text.text = d + " m";
        UIcolor = targetUI.color;
        if (distance <= 0.9)  //最大距離
        {
            UIcolor.a = 0.7058824f * ( (distance - 0.6f) * 5);  //透明度 * ((目前距離-最小距離)*5)
        }
        else
        {
            UIcolor.a = 0.7058824f;
        }
        targetUI.color = UIcolor;
        text.color = new Color(1, 1, 1, UIcolor.a);

        if (IsInView(MissionTaget[missionLevel].transform.position))
        {
            //Debug.Log("目前本物體在攝像機範圍內");
            Vector2 viewPos = Camera.WorldToViewportPoint(MissionTaget[missionLevel].transform.position);  //世界座標到視口座標
            Vector2 ScreenPos = Camera.ViewportToScreenPoint(viewPos);  //視口座標→螢幕座標
            Vector2 SP = new Vector2(ScreenPos.x - 960, ScreenPos.y - 540);
            //畫面左右邊界
            if (SP.x <= -900) SP.x = -900;
            else if (SP.x >= 900) SP.x = 900;
            //畫面上下邊界
            if (SP.y <= -350) SP.y = -350;
            else if (SP.y >= 350) SP.y = 350;
            targetUI.GetComponent<RectTransform>().anchoredPosition3D = SP;
        }
        else
        {
            Vector2 viewPos = Camera.WorldToViewportPoint(MissionTaget[missionLevel].transform.position);  //世界座標到視口座標
            Vector2 ScreenPos = Camera.ViewportToScreenPoint(viewPos);  //視口座標→螢幕座標
            Vector2 SP = new Vector2(ScreenPos.x - 960, ScreenPos.y - 540);

            //if (sp_Dot)  //背面
            //{
            //    sp_Dot_D = false;
            //    if (SP.y <= -350)
            //    {
            //        float tX = ScreenPos.x - 960;
            //        if (SP.x < tX - 130)
            //        {
            //            SP.x += 5000 * Time.deltaTime;
            //            SP = new Vector2(SP.x, SP.y);
            //            //print("1+++++" + SP);
            //        }
            //        else if (SP.x > tX + 130)
            //        {
            //            SP.x -= 5000 * Time.deltaTime;
            //            SP = new Vector2(SP.x, SP.y);
            //            //print("2-----" + SP);
            //        }
            //        else
            //        {
            //            SP = new Vector2(ScreenPos.x - 960, SP.y);
            //        }
            //    }
            //    else
            //    {
            //        SP.y -= 5000 * Time.deltaTime;
            //        //SP.x -= 0 * Time.smoothDeltaTime;
            //        SP = new Vector2(SP.x, SP.y);
            //        targetUI.GetComponent<RectTransform>().anchoredPosition3D = SP;
            //    }
            //}
            //else  //轉回正面
            //{
            //    if (sp_Dot_D)
            //    {
            //        SP = new Vector2(ScreenPos.x - 960, ScreenPos.y - 540);
            //        targetUI.GetComponent<RectTransform>().anchoredPosition3D = SP;

            //    }
            //    else
            //    {
            //        float tY = ScreenPos.y - 540;
            //        if (SP.y < tY - 130)
            //        {
            //            SP.y += 5000 * Time.deltaTime;
            //            SP = new Vector2(ScreenPos.x - 960, SP.y);
            //        }
            //        else
            //        {
            //            sp_Dot_D = true;
            //        }
            //    }
            //}            
            //畫面左右邊界
            if (SP.x <= -900) SP.x = -900;
            else if (SP.x >= 900) SP.x = 900;
            //畫面上下邊界
            if (SP.y <= -350) SP.y = -350;
            else if (SP.y >= 350) SP.y = 350;

            camTransform = Camera.transform; //相機座標
            Vector3 dirForward = (MissionTaget[missionLevel].transform.position - camTransform.position).normalized;
            dot = Vector3.Dot(camTransform.forward, dirForward);     //判斷物體是否在相機前面
            if (dot >= 0f)
            {
                targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(SP.x, SP.y, 0);
                sp_Dot = false;
                oldPos = new Vector2(SP.x, SP.y);
            }
            else if(dot < 0f)
            {
                Vector3 Bdir = MissionTaget[missionLevel].transform.position - camTransform.position; //位置差，方向  
                Rdot = Vector3.Dot(camTransform.right, Bdir.normalized);//點乘判斷左右： Rdot >0在右，<0在左
                Fdot = Vector3.Dot(camTransform.forward, Bdir.normalized);//點乘判斷前後：Fdot >0在前，<0在後
                //UI_y = 700 * Fdot;
                UI_y = -350;
                //if (UI_y <= -350) UI_y = -350;
                //else if (UI_y >= 350) UI_y = 350;
                sp_Dot = true;
                targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-SP.x, UI_y, 0);
                //targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-SP.x, SP.y, 0);
            }
            //else if(dot <0.2f && dot > -0.2f)
            //{
            //    Vector2 NviewPos = Camera.WorldToViewportPoint(this.gameObject.transform.position);  //世界座標到視口座標
            //    Vector2 NScreenPos = Camera.ViewportToScreenPoint(NviewPos);  //視口座標→螢幕座標
            //    Vector2 sPos = new Vector2(NScreenPos.x - 960, ScreenPos.y - 540);
            //    if (oldPos.x <= -900) sPos.x = -900;
            //    else if (oldPos.x >= 900) sPos.x = 900;
            //    //畫面上下邊界
            //    if (sPos.y <= -350) sPos.y = -350;
            //    else if (sPos.y >= 350) sPos.y = 350;
            //    targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(sPos.x, sPos.y, 0);
            //    print(targetUI.GetComponent<RectTransform>().anchoredPosition3D);
            //}

            ////Debug.Log("目前本物體不在攝像機範圍內");
            //Vector3 viewPos = Camera.WorldToViewportPoint(this.gameObject.transform.position);  //世界座標到視口座標
            //Vector3 tUIPosUp = targetUI.GetComponent<RectTransform>().up;
            //Vector3 tUIPosRight = targetUI.GetComponent<RectTransform>().right;

            //camTransform = Camera.transform; //相機座標
            //Vector3 dirForward = (transform.position - camTransform.position).normalized;
            //float dot = Vector3.Dot(camTransform.forward, dirForward);     //判斷物體是否在相機前面
            //Transform camT = Camera.transform;
            //camT.position = targetUI.GetComponent<RectTransform>().pivot;
            //Camdir = viewPos - camT.position; //位置差，方向  
            //                                       //方式1   點乘
            //                                       //點積的計算方式爲: a·b =| a |·| b | cos < a,b > 其中 | a | 和 | b | 表示向量的模 。
            //Rdot = Vector3.Dot(tUIPosRight, Camdir);//點乘判斷左右： Rdot >0在右，<0在左
            //Fdot = Vector3.Dot(tUIPosUp, Camdir);//點乘判斷前後：Fdot >0在前，<0在後
            //UI_x = 1920 * Rdot;
            //UI_y = 1080 * Fdot;
            ////畫面左右邊界
            //if (UI_x <= -900) UI_x = -900;
            //else if (UI_x >= 900) UI_x = 900;
            ////畫面上下邊界
            //if (UI_y <= -350) UI_y = -350;
            //else if (UI_y >= 350) UI_y = 350;

            //if (dot >= 0)
            //{            
            //    targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(UI_x, UI_y, 0);
            //}
            //else
            //{
            //    Vector3 Bdir = transform.position - camTransform.position; //位置差，方向  
            //    Rdot = Vector3.Dot(camTransform.right, Bdir.normalized);//點乘判斷左右： Rdot >0在右，<0在左
            //    Fdot = Vector3.Dot(camTransform.forward, Bdir.normalized);//點乘判斷前後：Fdot >0在前，<0在後

            //    //float posY = UI_y / UI_x;
            //    //UI_y = UI_y * posY;
            //    UI_x = 1920 * Rdot;
            //    UI_y = 1080 * Fdot;
            //    //畫面左右邊界
            //    if (UI_x <= -900) UI_x = -900;
            //    else if (UI_x >= 900) UI_x = 900;
            //    //畫面上下邊界
            //    if (UI_y <= -350) UI_y = -350;
            //    else if (UI_y >= 350) UI_y = 350;

            //    //targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(UI_x * -1, UI_y *-1, 0);
            //    targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(UI_x, UI_y, 0);
            //}
        }

        //if (Fdot > 0)
        //{
        //    print("前面");
        //}
        //else if (Fdot < 0)
        //{
        //    print("後面");
        //}
        //if (Rdot > 0)
        //{
        //    print("R右邊");
        //}
        //else if (Rdot < 0)
        //{
        //    print("L左邊");
        //}
    }
}
