using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    public GameObject Take;
    public GameObject AllObject;  //������
    public UpgradeValue[] �Z�����;  //(�Z������, �s��, �W��, �Ϥ�, ����, �¤O)
    public int DropdownType;
    public int FieldType;
    public int PartType;
    public static int ����ID;
    public string ����W��;
    public Text text;
    public static bool FirstWork;

    private void Awake()
    {
        Take = GameObject.Find("Take").gameObject;
    }
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
        ����ID = 0;
        ����W�� = "���ϥ�";
        time = -1;
        FirstWork = false;
        Shooting.UseWork(�Z�����);
    }

    void Update()
    {

        if (CamMove)
        {
            if (Move)  //�Ԫ�
            {
                if (time >= 0.9f || UpCamTransform.position == targetTransform[0].position)
                {
                    AllObject.SetActive(true);
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
            else  //�Ի�
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
            Shooting.UseWork(�Z�����);
        }
        if (Input.GetKeyDown(KeyCode.E))  //�� [E] ���}
        {
            if (Move)
            {
                Exit();
                UpgradeMenu.SetActive(false);
            }
        }

        for (int i = 0; i < �Z�����.Length; i++)  //�Z�����
        {
            for (int n = 0; n < �Z�����[i].����.Length; n++) //�Z��
            {
                for (int m = 0; m < �Z�����[i].����[n].Part.Length; m++)  //�Z������
                {
                    if (�Z�����[i].����[n].Part[m] != null)
                    {
                        �Z�����[i].����[n].ID[m] = m;
                        �Z�����[i].����[n].PartName[m] = �Z�����[i].����[n].Part[m].name;
                    }
                }
            }
        }
        switch (����ID)
        {
            case 0:
                text.text = "["+����W��+"]\n�����������o���P�ĪG";
                break;
            case 1:
                text.text = "[" + ����W�� + "]\n�୰�C�}���ɪ�����";
                break;
            case 2:
                text.text = "[" + ����W�� + "]\n�୰�C�g���᪺��y�O";
                break;
        }
    }
    public void UseType(int Type)
    {
        DropdownType = Type;
    }
    public void UsePartType(float Type)  //(0.0) 1���� 0.1�Z�����
    {
        PartType = (int)Type;  //��������
        FieldType = (int)(Type - PartType) * 10;  //�Z���������
    }
    public void UseDropdown(Dropdown dropdown)
    {
        switch (DropdownType)
        {
            case 0:  //���Z��
                for (int i = 0; i < �Z�����.Length; i++)
                {
                    �Z�����[i].Object.SetActive(false);
                    for (int n = 0; n < �Z�����[i].����.Length; n++)
                    {
                        �Z�����[i].����[n].PartObject[1].SetActive(false);
                    }
                }
                if (!�Z�����[dropdown.value].Object.activeSelf)
                {
                    �Z�����[dropdown.value].Object.SetActive(true);
                    for(int n=0; n< �Z�����[dropdown.value].����.Length; n++)
                    {
                        �Z�����[dropdown.value].����[n].PartObject[1].SetActive(true);
                    }
                }
                break;
            case 1:  //������
                for (int i = 0; i < �Z�����[FieldType].����[PartType].Part.Length; i++)
                {
                    �Z�����[FieldType].����[PartType].Part[i].SetActive(false);
                }
                if (!�Z�����[FieldType].����[PartType].Part[dropdown.value].activeSelf)
                {
                    �Z�����[FieldType].����[PartType].Part[dropdown.value].SetActive(true);
                    �Z�����[FieldType].Power = �Z�����[FieldType].����[PartType].Power[dropdown.value];  //�Z���ˮ`
                    �Z�����[FieldType].Recoil = �Z�����[FieldType].����[PartType].Recoil[dropdown.value];  //�Z����y�O
                    ����ID = �Z�����[FieldType].����[PartType].ID[dropdown.value];
                    ����W�� = �Z�����[FieldType].����[PartType].PartName[dropdown.value];
                    Shooting.������ = true;
                }
                break;
        }   
    }
    public void UseButton(int Type)
    {

    }

    public void Exit( )  //���}
    {
        AllObject.SetActive(false);  //���������ɯų���
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
            if (!FirstWork)
            {
                FirstWork = true;
                DialogueEditor.StartConversation(0, 4, 0, true, 0);  //�}�l���
            }
        }
    }
}
[Serializable]
public class UpgradeValue
{
    public String ����;  //�Z���W��
    public GameObject Object;  //�Z��
    public ����[] ����;  //�Z������
    public int Lvevl;  //����
    public float Power;  //�¤O
    public float Recoil;  //��y�O
    public float Price; //����

    /// <summary>
    /// �U�Z���ƭ�
    /// </summary>
    /// <param name="����">�Z������</param>
    /// <param name="Object">�Z��</param>
    /// <param name="�Z������">�Z������</param>
    /// <param name="Lvevl">����</param>
    /// <param name="Power">�¤O</param>
    /// <param name="Price">����</param>
    /// <returns></returns>
    public UpgradeValue(String _����, GameObject gameObject, ����[] _����, int lvevl, float power, float recoil, float price)
    {
        ���� = _����;
        Object = gameObject;
        ���� = _����;
        Lvevl = lvevl;
        Power = power;
        Recoil = recoil;
        Price = price;
    }
}
[Serializable]
public class ����
{
    public String ����;  //�Z���W��
    public GameObject[] PartObject;  //�Z������
    public int[] ID; //�s��
    public String[] PartName; //����W��
    public GameObject[] Part;  //�Z������
    public int[] Lvevl;  //����
    public float[] Power;  //�¤O
    public float[] Recoil;  //��y�O
    public float[] Price; //����

    /// <summary>
    /// �U�Z���ƭ�
    /// </summary>
    /// <param name="����">�Z������</param>
    /// <param name="Object">�Z��</param>
    /// <param name="ID">�s��</param>
    /// <param name="PartName">�Z������</param>
    /// <param name="Part">�Z������</param>
    /// <param name="Lvevl">����</param>
    /// <param name="Power">�¤O</param>
    /// <param name="Price">����</param>
    /// <returns></returns>
    public ����(String _����, GameObject[] partObject, int[] id, String[] partName, GameObject[] part, int[] lvevl, float[] power, float[] recoil, float[] price)
    {
        ���� = _����;
        ID = id;
        PartName = partName;
        PartObject = partObject;
        Part = part;
        Lvevl = lvevl;
        Power = power;
        Recoil = recoil;
        Price = price;
    }
}