using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackUtility
{
    public int AttackTargets(AttackLevel AttackLv, Transform Attacker,string[] targetTags, LayerMask ActorLayer)
    {
        int count = 0;

        // 取得範圍內所有角色

        Collider[] actors = Physics.OverlapSphere(Attacker.position, AttackLv.distance, ActorLayer);
        for (int i = 0; i < actors.Length; i++)
        {
            bool isTarget = false;

            for (int pt = 0; pt < targetTags.Length; pt++) // 判斷該角色是否為目標
            {
                if (actors[i].tag == targetTags[pt])
                {
                    isTarget = true;
                    break;
                }
            }

            if (isTarget) // 若角色是目標
            {
                // 若在視線角度內
                if (GetXZAngle(Attacker.forward, Attacker.position, actors[i].transform.position, false) < AttackLv.angle)
                {
                    if (NoObstacle(Attacker, actors[i].transform, AttackLv.height, 0xFFFF - ActorLayer)) // 若中間沒有障礙物
                    {
                        if (actors[i].gameObject.layer == LayerMask.NameToLayer("Actor"))
                        {
                            actors[i].transform.SendMessage("Damage", AttackLv.power); // 進行傷害
                        }
                    }
                }
            }
        }

        return count;
    }

    // 用射線判斷武器到目標間是否有障礙物
    bool NoObstacle(Transform Attacker, Transform targetActor, float WeaponHeight, LayerMask ObstacleLayer)



    {
        bool ret = true;

        // 打一條射線看之間是否有障礙物
        Vector3 origin = Attacker.position + new Vector3(0, WeaponHeight, 0);
        Vector3 targetPos = targetActor.position + new Vector3(0, WeaponHeight, 0);
        Vector3 direct = targetPos - origin;
        float distance = Vector3.Distance(Attacker.position, targetActor.position);
        int maskMonster = 1 << LayerMask.NameToLayer("Monster");
        int maskNoShoot = 1 << LayerMask.NameToLayer("NoShoot");
        Ray ray = new Ray(origin, direct);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, distance, ObstacleLayer)) // 若有打到東西代表有障礙物
        {
#if UNITY_EDITOR
            Debug.DrawRay(origin, hit.point - origin, Color.yellow, 0.5f);
#endif
            if (Physics.Raycast(ray, out hit, distance, maskMonster))  //無視Monster圖層
            {

            }
            else if (Physics.Raycast(ray, out hit, distance, maskNoShoot))
            {

            }
            else
            {
                // 射線有打到東西表示角色間有障礙物
                ret = false;
            }
        }

        return ret;
    }


    /// <summary>
    /// 計算自己與對象的夾角
    /// </summary>
    /// <param name="forward">自己的前方向量</param>
    /// <param name="selfPosition">自已的座標</param>
    /// <param name="targetPosition">對象的座標</param>
    /// <param name="signedNumber">若為 true 表示有號數,負值表示對象在左邊,fasle 則不分左右</param>
  /// <returns>-180~+180 的角度</returns>
  public float GetXZAngle(Vector3 forward, Vector3 selfPosition, Vector3
targetPosition, bool signedNumber)
    {



        Vector3 targetdir = targetPosition - selfPosition;

        // 計算夾角
        float angle = Vector2.Angle(new Vector2(forward.x, forward.z), new
      Vector2(targetdir.x, targetdir.z));

        if (signedNumber) // 若需區分左右
        {
            Vector3 crossV = Vector3.Cross(forward, targetdir); // 外積
            if (crossV.y < 0) // 若目標在左邊
                angle *= -1;
        }

        return angle;
    }
}

[Serializable]
public class AttackLevel
{
    public bool displayGizmos; // 是否顯示 Gizmos
    public float power; // 威力
    public float distance; // 距離
    public float angle; // 角度
    public float height;// 高度

    public AttackLevel(bool DiaplayGizmos, float Power, float Distance, float Angle, float Height)
    {
        displayGizmos = DiaplayGizmos;
        power = Power;
        distance = Distance;
        angle = Angle;
        height = Height;
    }
}
