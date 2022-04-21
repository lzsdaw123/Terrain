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
    public bool LightB=true;  //�O�_�����O��
    public bool ShadowsB=false;   //�O�_�������v
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
        camTransform = Gun_Camera.transform;  //�۾��y��
        Vector2 viewPos = Gun_Camera.WorldToViewportPoint(worldPos);  //�@�ɮy�Ш���f�y��
        Vector3 dir = (worldPos - camTransform.position).normalized;
        c_dot = Vector3.Dot(camTransform.forward, dir);     //�P�_����O�_�b�۾��e��

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
        Vector2 vec2 = Gun_Camera.WorldToScreenPoint(this.gameObject.transform.position);  //�@�ɮy�Ш�ù��y��
        if (camTransform == null || Gun_Camera == null)
        {
            camTransform = GameObject.Find("Gun_Camera").GetComponent<Transform>();
            Gun_Camera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        }
        distance = (camTransform.position - transform.position).magnitude;
        _IsInView = IsInView(transform.position);

        if (IsInView(transform.position))  //�b������
        {
            if (distance <= minDistance[Type])  //�b�d��
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
            else if (distance >= MaxDistance[Type])  //�b�d�򤧥~
            {
                off(LightB, ShadowsB);
            }
        }
        else if (!IsInView(transform.position))  //���b������
        {
            if (distance <= minDistance[Type])   //�b�d��
            {
                HDAdditionalLightData.SetShadowResolution(256);
                //Light.enabled = true;
                On(LightB, ShadowsB);
            }
            else if (distance > minDistance[Type] && distance < MaxDistance[Type])  //�b�d��w�Ĥ�
            {
                //HDAdditionalLightData.SetShadowResolution(128);
                On(LightB, ShadowsB);
            }
            else if (distance >= MaxDistance[Type]) //�b�d�򤧥~
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
            HDAdditionalLightData.EnableShadows(true);  //�}�ҳ��v
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
            HDAdditionalLightData.EnableShadows(false);  //�������v
        }
    }
}
