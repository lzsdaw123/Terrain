using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MG_Turret_AI : MonoBehaviour
{
    public ObjectPool pool;
    [SerializeField] private AnimEvents AnimEvents;
    public Crystal_Life crystal_Life;
    public Boss_Life boss_Life;
    public Animator ani; //�ʵe���
    public GameObject Player;  //���a
    public GameObject bullet;
    public LayerMask layerMask;  //�ϼh
    [SerializeField] private bool attacking;
    private int bulletAttack;
    public static int BulletNub;  //�l�u�ƶq
    public float power;  //�ˮ`
    [SerializeField] int SF_BulletNub;  //�l�u�ƶq
    [SerializeField] private GameObject[] muzzle;  //�j�f
    public GameObject[] RigTarget;  //�j�f�˷ǥؼ�
    public GameObject[] MA_Rig;  //Rig�s��
    public float MA_weight;  //Rig�s���v��
    public bool StartAttack;  //�i�J�������A
    public int AttackRange;  //�����d��
    public bool[] InAttackRange;
    public float LockTime;  //��w�ɶ�
    public bool Reload;
    public float ReloadTime;
    public bool overheatLock;  //�L����w
    public float coolDown;
    public bool Fire;
    public bool[] FlyStart;  //�l�u�}�l����
    public float[] FlySpeed; //�l�u����t��
    public float[] distance; //�l�u�w�p����Z��
    public bool[] SureHit;  //���X�g�u
    public Vector3[] O_targetPos;
    public Vector3 NO_targetPos;
    public float[] ETA;  //�w�p��F�ɶ�
    public bool[] HitTarget;  //�R�����a
    public bool Dead;

    void Start()
    {
        //pool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        attacking = false;
        bulletAttack = 1;
        //Fire = false;
        //AttackStatus = false;
        coolDown = 1;
        BulletNub = 30;  //�l�u��
        power = 0.5f;
        StartAttack = false;
        Dead = false;
        ReloadTime = 0;
        SureHit = new bool[] { false, false };
        FlyStart = new bool[] { false, false };
        HitTarget = new bool[] { false, false };
        InAttackRange = new bool[] { true, false };
    }

    void Update()
    {
        Dead = crystal_Life.Dead;
        if (boss_Life.Dead || !boss_Life.gameObject.GetComponent<Boss02_AI>().enabled) StartAttack = false;

        if (StartAttack && !Dead)
        {
            SF_BulletNub = BulletNub;

            ani.SetBool("Start", true);  //�i�J�����Ҧ�
            Vector3 origin = muzzle[0].transform.position;
            Vector3 targetPos = Player.transform.position + new Vector3(0, 2, 0);
            Vector3 direct = targetPos - origin;
            Ray ray = new Ray(origin, direct);
            RaycastHit hit = new RaycastHit(); //�g�u������T

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.1f, false);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //���o���a
                {
                    if (hit.collider.tag == "Player")  //���a
                    {

                        //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.1f, false);
                        if (BulletNub <= 0)   //�Z��1 �L��
                        {
                            Reload = true; //�N�o���A
                            overheatLock = true;   // �L����w
                            BulletNub = 0;
                            bulletAttack = 0;
                            ReLoad();
                            attacking = false;
                            //print("BulletNub =0");
                        }
                        else  //�Z��1 ����
                        {
                            LockTime = 0;
                            Fire = true;
                            attacking = true;
                            if(!overheatLock) Reload = false;  //�D��w���A ���}ReLoad
                            Attack();
                            //ani.SetBool("Attack", true);  //����
                            //print("����");
                        }
                    }
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))  //���a���b����
                {
                    if (hit.collider.tag == "Crystal")  //����
                    {
                        //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 0.4f, false);
                        HitTarget[0] = false;
                        HitTarget[1] = false;
                        LockTime += Time.deltaTime;
                        if (LockTime >= 0.1f)
                        {
                            //AttackMode = 2;   //�����Ҧ�2
                            LockTime = 1;
                            overheatLock = false;   // �L����w
                             //bulletAttack = 0;
                            Reload = true;
                            ReLoad();
                            //print("�Q�צ�F");
                        }
                    }
                }
            }
            if (InAttackRange[0] && InAttackRange[1]) AttackRange = 0;  //�b�����~&�ж��� =0
            if (!InAttackRange[0] && InAttackRange[1]) AttackRange = 1;  //�b������&�ж��� =1
            if (InAttackRange[0] && !InAttackRange[1]) AttackRange = 1;  //�b�����~&�ж��~ =1

            switch (AttackRange)
            {
                case 0:  //�d��
                    if (Reload && overheatLock)  //�B��N�o���A �ùL����w
                    {
                        //AttackMode = 2;
                        //print("�L����");
                    }
                    if (Reload && !overheatLock)  //�B��N�o���A �� �D�L����w
                    {
                        Reload = false;
                        //AttackMode = 1;  //�����Ҧ�1
                        //print("�L�����}��");
                    }
                    else if (!Reload)  //�B��D�N�o���A
                    {
                        //AttackMode = 1;  //�����Ҧ�1
                        //print("�}��");
                    }
                    MA_weight += 0.5f * Time.deltaTime;
                    if (MA_weight >= 1) MA_weight = 1;
                    break;
                case 1:  //�d��~����
                    Reload = true;
                    overheatLock = false;
                    ReLoad();
                    MA_weight -= 0.5f * Time.deltaTime;
                    if (MA_weight <= 0) MA_weight = 0f;
                    break;
            }

            MA_Rig[0].GetComponent<MultiAimConstraint>().weight = MA_weight;  //�j�f�s��
            MA_Rig[1].GetComponent<MultiAimConstraint>().weight = MA_weight;  //�j�f�s��
            //ani.SetInteger("AttackMode", AttackMode);
        }
        else
        {
            ani.SetBool("Start", false);  //�i�J�D�����Ҧ�
        }
        MA_Rig[0].GetComponent<MultiAimConstraint>().weight = MA_weight;  //�j�f�s��

    }
    void ReLoad()  //�Z���N�o
    {
        if (Reload)
        {
            ani.SetBool("Attack", false);  //������
            ReloadTime += Time.deltaTime;
            if (ReloadTime >= 2)
            {
                ReloadTime = 0;
                Reload = false;
                BulletNub = 30;
            }
            //MuzzleMaterial.SetFloat("_EmissiveExposureWeight", overheatTime);
        }
    }
    void FixedUpdate()
    {
        if (!StartAttack) return;
        if (bulletAttack >= 1)
        {
            attacking = false;
            bulletAttack = 0;
        }
        if (FlyStart[0])  //�Ĥ@�o�l�u
        {
            if (!SureHit[0])
            {
                RigTarget[0].transform.position = Player.transform.position + new Vector3(0, 2f, 0);
                Vector3 O_origin = muzzle[0].transform.position;
                if (!HitTarget[0])
                {
                    O_targetPos[0] = RigTarget[0].transform.position;
                }
                Vector3 O_direct = O_targetPos[0] - O_origin;
                Ray O_ray = new Ray(O_origin, O_direct);
                RaycastHit O_hit = new RaycastHit(); //�g�u������T
                if (Physics.Raycast(O_ray, out O_hit, 100f, layerMask))
                {
                    distance[0] = Vector3.Distance(O_ray.origin, O_hit.point);  //���o�P�ؼжZ��
                    if (!HitTarget[0])
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.green, 0.1f, false);  //��
                        FlySpeed[0] = 0;
                        SureHit[0] = true;
                    }
                    else
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.red, 0.1f, false);  //��
                        if (O_hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //���o���a
                        {
                            if (O_hit.collider.tag == "Player")  //���a
                            {
                                O_hit.transform.SendMessage("Damage", power);  //�y���ˮ`
                                O_hit.collider.gameObject.SendMessage("hit_Direction", transform); //�R�����
                            }
                        }
                        FlyStart[0] = false;
                        HitTarget[0] = false;
                    }
                }
            }
            else
            {
                if (FlySpeed[0] >= 0)
                {
                    ETA[0] = 0.0018f * distance[0];  //�w�p��F�ɶ� = ((�C1�Z��/ 0.0018�� ) * �ؼжZ�� ) +�}������
                    FlySpeed[0] += Time.deltaTime;
                    if (FlySpeed[0] >= ETA[0])
                    {
                        FlySpeed[0] = -1;
                        HitTarget[0] = true;
                        SureHit[0] = false;
                    }
                }
            }
        }
        if (FlyStart[1])  //�ĤG�o�l�u
        {
            if (!SureHit[1])
            {
                RigTarget[0].transform.position = Player.transform.position + new Vector3(0, 2f, 0);  //�ؼФ@��
                Vector3 O_origin = muzzle[1].transform.position;
                if (!HitTarget[1])
                {
                    O_targetPos[1] = RigTarget[0].transform.position;
                }
                Vector3 O_direct = O_targetPos[1] - O_origin;
                Ray O_ray = new Ray(O_origin, O_direct);
                RaycastHit O_hit = new RaycastHit(); //�g�u������T
                if (Physics.Raycast(O_ray, out O_hit, 100f, layerMask))
                {
                    distance[1] = Vector3.Distance(O_ray.origin, O_hit.point);  //���o�P�ؼжZ��
                    if (!HitTarget[1])
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.black, 0.1f, false);  //��
                        FlySpeed[1] = 0;
                        SureHit[1] = true;
                    }
                    else
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.blue, 0.1f, false);  //��
                        if (O_hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //���o���a
                        {
                            if (O_hit.collider.tag == "Player")  //���a
                            {
                                O_hit.transform.SendMessage("Damage", power);  //�y���ˮ`
                                O_hit.collider.gameObject.SendMessage("hit_Direction", transform); //�R�����
                            }
                        }
                        FlyStart[1] = false;
                        HitTarget[1] = false;
                    }
                }
            }
            else
            {
                if (FlySpeed[1] >= 0)
                {
                    ETA[1] = 0.0018f * distance[1];  //�w�p��F�ɶ� = ((�C1�Z��/ 0.0018�� ) * �ؼжZ�� ) +�}������
                    FlySpeed[1] += Time.deltaTime;
                    if (FlySpeed[1] >= ETA[1])
                    {
                        FlySpeed[1] = -1;
                        HitTarget[1] = true;
                        SureHit[1] = false;
                    }
                }
            }
        }
    }
    private void Attack()
    {
        if (!StartAttack) return;
        //switch (AttackMode)
        //{
        //    case 1:
        //        if (BulletNub <= 18)
        //        {
        //            float overheat = 0.91f + BulletNub * (0.09f / 18);  //�̤pEEW + ��e�l�u�� * (�̤p�P�̤jEEW�t�� / �l�u�W��) 
        //            if (overheat <= 0.91f) overheat = 0.91f;
        //            MuzzleMaterial.SetFloat("_EmissiveExposureWeight", overheat);
        //        }
        //        //print("�l�u " + BulletNub);
        //        break;
        //    case 2:
        //        //ani.SetBool("Attack1", false);
        //        //print("����2");
        //        break;
        //}
        //ani.SetInteger("AttackMode", AttackMode);

        if (Fire)
        {
            ReloadTime = 0;
            ani.SetBool("Attack", true);  //����
            //print("Attack");
        }
    }
    public void AttackAning(bool attackingB, int BulletAttackNub)
    {
        attacking = attackingB;
        bulletAttack = BulletAttackNub;
    }
    void OnDisable()  //�T�ή�
    {
        attacking = false;
    }
}
