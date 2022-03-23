using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWorkbench : MonoBehaviour
{
    public GameObject TextG;
    public int Type;
    public Shop Shop; 
    public GameObject UpgradeMenu;  //�ɯ�UI
    public GameObject play;
    public Camera GunCamera;  //���a�۾�
    public Camera UpgradeCamera;  //�ɯŬ۾�
    public Transform GunCamTransform;  //���a�۾���m
    public Transform UpCamTransform;  //�ɯŬ۾���m
    public Transform[] targetTransform;  //���w��m
    public bool CamMove;  //�O�_�۾��첾
    public bool Move;  //�۾��첾�P�k��
    private Vector3 currentVelocity = Vector3.zero;     // ��e�t�סA�o�ӭȥѧA�C���I�s�o�Ө禡�ɳQ�ק�
    float maxSpeed = 40f;    // ��ܤ��\�A����̤j�t��
    float smoothTime = 0.25f;      // �F��ؼФj����O���ɶ��C �@�Ӹ��p���ȱN��ֹF��ؼСC
    public float time;  //�첾�ɶ�
    public Vector3 tagTranPos;
    public Quaternion tagTranQu;
    float FieldOfView;  //�۾�����
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
                    Cursor.lockState = CursorLockMode.None; //��еL���A�Ҧ�
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
        Cursor.lockState = CursorLockMode.Locked; //�����w�Ҧ�

    }
    void HitByRaycast() //�Q�g�u����ɷ|�i�J����k
    {
        TextG.GetComponent<Text>().text = "���uE�v�ϥΤu�@�O";
        QH_interactive.thing();  //�I�sQH_�B���Ϯ�

        if (Input.GetKeyDown(KeyCode.E)) //����U��L E ���
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
