using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 將指令碼掛在攝像機觀察的物體上  物體必須帶有Render
/// </summary>
public class PlayerView : MonoBehaviour
{
    bool isRendering;
    float curtTime = 0f;
    float lastTime = 0f;
    public Camera Camera;
    public Transform target;
    public float Rdot;
    public float Fdot;
    public Image targetUI;
    public float UI_x;
    public float UI_y;

    void OnWillRenderObject()
    {
        curtTime = Time.time;
    }
    public bool IsInView(Vector3 worldPos)
    {
        Transform camTransform = Camera.transform;  //相機座標
        Vector2 viewPos = Camera.WorldToViewportPoint(worldPos);  //世界座標到視口座標
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判斷物體是否在相機前面


        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }
    void Update()
    {
        Vector2 vec2 = Camera.WorldToScreenPoint(this.gameObject.transform.position);  //世界座標到螢幕座標

        if (IsInView(transform.position))
        {
            //Debug.Log("目前本物體在攝像機範圍內");
        }
        else
        {
            //Debug.Log("目前本物體不在攝像機範圍內");
        }
        target = Camera.transform;
        Vector3 dir = transform.position - target.position; //位置差，方向  
        //方式1   點乘  
        //點積的計算方式爲: a·b =| a |·| b | cos < a,b > 其中 | a | 和 | b | 表示向量的模 。  

        Rdot = Vector3.Dot(target.right, dir.normalized);//點乘判斷左右： dot1>0在右，<0在左
        Fdot = Vector3.Dot(target.forward, dir.normalized);//點乘判斷前後：dot >0在前，<0在後

        UI_x = 1920 * Rdot;
        UI_y = 1080 * Fdot;

        if(UI_x<=-900)  UI_x = -900;
        else if(UI_x >= 900) UI_x = 900;

        if (UI_y <= -350) UI_y = -350;
        else if (UI_y >= 350) UI_y = 350;

        //60 /1860
        //190 /890
        if (Fdot > 0)
        {
            print("前面");
        }
        else if (Fdot < 0)
        {
            print("後面");
        }
        if (Rdot > 0)
        {
            print("R右邊");
        }
        else if (Rdot < 0)
        {
            print("L左邊");
        }
        targetUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(UI_x, UI_y, 0);
    }
}
