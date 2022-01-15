using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoving : StateMachineBehaviour
{
    GameObject player;
    Rigidbody rb;

    BossAI bossAI;
    public float bossMoveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = animator.GetComponent<Rigidbody>();
        bossAI = animator.GetComponent<BossAI>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 movingTowPlayer = Vector3.MoveTowards(animator.transform.position, player.transform.position, bossMoveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(movingTowPlayer);
        bossAI.LookAtPlayer();

        if (bossAI.weaponCD)
            bossMoveSpeed = 9;
        else
            bossMoveSpeed = 3;

        if(!bossAI.canAttackwWeapon)
            bossAI.mainBody.transform.rotation = Quaternion.RotateTowards(bossAI.mainBody.transform.rotation, animator.transform.rotation, 2);
    }
}
