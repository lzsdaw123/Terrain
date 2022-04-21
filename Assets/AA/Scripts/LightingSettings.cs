using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightingSettings : MonoBehaviour
{
    public int Type;
    float curtTime = 0f;
    float lastTime = 0f;
    public Transform camTransform;
    public Camera Gun_Camera;
    public float distance;
    public bool Low;
    public bool _IsInView;
    public bool LightB=true;  //是否關閉燈光
    public bool ShadowsB=false;   //是否關閉陰影
    public float c_dot;
    [SerializeField] Vector2 _viewPos;
    [SerializeField] float[] minDistance;
    [SerializeField] float[] MaxDistance;
    public Light Light;
    public HDAdditionalLightData HDAdditionalLightData;

    void Awake()
    {
        camTransform = GameObject.Find("Gun_Camera").GetComponent<Transform>();
        Gun_Camera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        Light = GetComponent<Light>(); ;
        HDAdditionalLightData = GetComponent<HDAdditionalLightData>(); ;
    }
    void Start()
    {
        minDistance = new float[] { 7, 105, 40 };
        MaxDistance = new float[] { 7.7f, 110f, 60 };
    }
    void OnWillRenderObject()
    {
        curtTime = Time.time;
    }
    public bool IsInView(Vector3 worldPos)
    {
        if (camTransform == null || Gun_Camera==null) 
        {
            camTransform = GameObject.Find("Gun_Camera").GetComponent<Transform>();
            Gun_Camera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        }
        camTransform = Gun_Camera.transform;  //相機座標
        Vector2 viewPos = Gun_Camera.WorldToViewportPoint(worldPos);  //世界座標到視口座標
        Vector3 dir = (worldPos - camTransform.position).normalized;
        c_dot = Vector3.Dot(camTransform.forward, dir);     //判斷物體是否在相機前面

        _viewPos = viewPos;

        //if (c_dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
        //    return true;
        //else
        //    return false;
        if (c_dot > -0.1)
            return true;
        else
            return false;
    }
    void Update()
    {
        Vector2 vec2 = Gun_Camera.WorldToScreenPoint(this.gameObject.transform.position);  //世界座標到螢幕座標
        if (camTransform == null || Gun_Camera == null)
        {
            camTransform = GameObject.Find("Gun_Camera").GetComponent<Transform>();
            Gun_Camera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        }
        distance = (camTransform.position - transform.position).magnitude;
        _IsInView = IsInView(transform.position);

        if (IsInView(transform.position))  //在視角內
        {
            if (distance <= minDistance[Type])  //在範圍內
            {
                if (Low)
                {
                    HDAdditionalLightData.SetShadowResolution(256);
                }
                else
                {
                    HDAdditionalLightData.SetShadowResolution(512);
                }
                On(LightB, ShadowsB);
            }
            else if (distance >= MaxDistance[Type])  //在範圍之外
            {
                off(LightB, ShadowsB);
            }
        }
        else if (!IsInView(transform.position))  //不在視角內
        {
            if (distance <= minDistance[Type])   //在範圍內
            {
                HDAdditionalLightData.SetShadowResolution(256);
                //Light.enabled = true;
                On(LightB, ShadowsB);
            }
            else if (distance > minDistance[Type] && distance < MaxDistance[Type])  //在範圍緩衝內
            {
                //HDAdditionalLightData.SetShadowResolution(128);
                On(LightB, ShadowsB);
            }
            else if (distance >= MaxDistance[Type]) //在範圍之外
            {
                off(LightB, ShadowsB);
            }
        }
    }

    void On(bool light, bool shadows)
    {
        if (light)
        {
            this.Light.enabled = true;
        }
        if (shadows)
        {
            HDAdditionalLightData.EnableShadows(true);  //開啟陰影
        }

    }
    void off(bool light, bool shadows)
    {
        if (light)
        {
            this.Light.enabled = false;
        }
        if (shadows)
        {
            HDAdditionalLightData.EnableShadows(false);  //關閉陰影
        }
    }
}
