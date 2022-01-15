using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : StateMachineBehaviour
{
    GameObject player;
    Transform weapon;
    Transform mainBody;

    BossAI bossAI;

    float weaponAtkDuration;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossAI = animator.GetComponent<BossAI>();
        player = GameObject.FindGameObjectWithTag("Player");
        weapon = animator.transform.Find("EnemyModel/MainBody/Arm1/Weapon/Pipe/Muzzle");
        mainBody = animator.transform.Find("EnemyModel/MainBody");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (bossAI.canAttackwWeapon)
        {
            weaponAtkDuration += Time.deltaTime;

            if (weaponAtkDuration >= 3)
            {
                weaponAtkDuration = 0;
                bossAI.canAttackwWeapon = false;
                bossAI.weaponCD = true;
            }

            weapon.transform.LookAt(player.transform.position);
            Vector3 dir = mainBody.transform.position - player.transform.position;
            Quaternion lookDir = Quaternion.LookRotation(dir, Vector3.up);

            lookDir.x = 0;
            lookDir.z = 0;

            mainBody.transform.rotation = Quaternion.RotateTowards(mainBody.transform.rotation, lookDir, 2);
        }

        if (bossAI.canAttackwFist)
        {
            Vector3 dir = mainBody.transform.position - player.transform.position;
            Quaternion lookDir = Quaternion.LookRotation(dir, Vector3.up);

            lookDir.x = 0;
            lookDir.z = 0;

            mainBody.transform.rotation = Quaternion.RotateTowards(mainBody.transform.rotation, lookDir, 2);
        }
    }
}
