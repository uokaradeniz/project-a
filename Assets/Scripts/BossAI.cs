using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class BossAI : MonoBehaviour
{
    GameObject player;
    Rigidbody rb;

    public bool canAttackwWeapon;
    public bool canAttackwFist;
    public bool canAttackBSA1;
    public float rotSpeed;
    public float weaponAttackRange;
    public float ccAttackRange;

    public int bossHealth;

    public bool weaponCD;

    [HideInInspector]public int bossBSAAtkDamage;
    public int bossBSADmgMin;
    public int bossBSADmgMax;

    [HideInInspector]public bool bsaAttack;

    [HideInInspector]public Transform mainBody;
    Animator animator;

    Transform weapon;

    int bossCCAttackDamage;
    public int bossCCDmgMin;
    public int bossCCDmgMax;

    float bsaTimer;
    public float BSACooldown;
    public float bsaAttackrange;

    public bool bossAIActive;


    // Start is called before the first frame update
    void Start()
    {
        weapon = transform.Find("EnemyModel/MainBody/Arm1/Weapon/Pipe/Muzzle");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        mainBody = transform.Find("EnemyModel/MainBody");

    }

    private void Update()
    {
        if (bossHealth <= 0)
        {
            GameObject.Find("SystemSettings").GetComponent<GameBehaviour>().bossIsDead = true;           
            GameObject.Destroy(this.gameObject, 0.2f);
        }

        if (bossAIActive)
        {
            bossBSAAtkDamage = Random.Range(bossBSADmgMin, bossBSADmgMax);
            bossCCAttackDamage = Random.Range(bossCCDmgMin, bossCCDmgMax);
            bsaTimer += Time.deltaTime;

            if (bsaAttack)
            {
                animator.SetTrigger("BSA1");
                canAttackwFist = false;
                canAttackwWeapon = false;
            }

            if (canAttackBSA1)
            {
                if (bsaTimer > 10)
                {
                    bsaAttack = true;
                    bsaTimer = 0;
                }
            }

            if (weaponCD)
                Invoke("CancelWeaponCD", 4);

            if (!player.GetComponent<PlayerHealth>().playerIsDead)
            {
                if (canAttackwWeapon && !weaponCD)
                    animator.SetBool("BossAttackingwWeapon", true);
                else
                    animator.SetBool("BossAttackingwWeapon", false);

                if (canAttackwFist && !bsaAttack)
                    animator.SetBool("BossAttackingwFist", true);
                else
                    animator.SetBool("BossAttackingwFist", false);

                float distBetweenPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (!weaponCD)
                {
                    if (distBetweenPlayer < weaponAttackRange && distBetweenPlayer > ccAttackRange)
                    {
                        animator.SetBool("BossIsMoving", false);
                        canAttackwFist = false;
                        canAttackwWeapon = true;
                        canAttackBSA1 = false;
                    }
                    else if (distBetweenPlayer < ccAttackRange && distBetweenPlayer > bsaAttackrange)
                    {
                        animator.SetBool("BossIsMoving", false);
                        canAttackwWeapon = false;
                        canAttackwFist = true;
                        canAttackBSA1 = false;
                    }
                    else if (distBetweenPlayer >= weaponAttackRange)
                    {
                        canAttackwFist = false;
                        canAttackwWeapon = false;
                        canAttackBSA1 = false;
                        animator.SetBool("BossIsMoving", true);
                    }
                    else if (distBetweenPlayer < bsaAttackrange && bsaTimer > BSACooldown)
                    {
                        animator.SetBool("BossIsMoving", false);
                        canAttackwWeapon = false;
                        canAttackwFist = false;
                        canAttackBSA1 = true;
                    }
                    else if (distBetweenPlayer < bsaAttackrange && bsaTimer < BSACooldown)
                    {
                        canAttackBSA1 = false;
                        canAttackwFist = true;
                    }
                }
                else
                {
                    if (distBetweenPlayer > bsaAttackrange + 1f)
                    {
                        animator.SetBool("BossIsMoving", true);
                    }
                    else if (distBetweenPlayer < bsaAttackrange + 1 && distBetweenPlayer > bsaAttackrange)
                    {
                        animator.SetBool("BossIsMoving", false);
                        canAttackwWeapon = false;
                        canAttackwFist = true;
                        canAttackBSA1 = false;
                    }
                    else if (distBetweenPlayer < bsaAttackrange && bsaTimer > BSACooldown)
                        canAttackBSA1 = true;
                    else if (distBetweenPlayer < bsaAttackrange && bsaTimer < BSACooldown)
                    {
                        canAttackwFist = true;
                        canAttackBSA1 = false;
                    }
                }
            }
            else
            {
                animator.SetBool("BossAttackingwFist", false);
                animator.SetBool("BossAttackingwWeapon", false);
                animator.SetBool("BossIsMoving", false);
            }
        }
    }

    public void LookAtPlayer()
    {
        Vector3 dir = transform.position - player.transform.position;
        Quaternion lookDir = Quaternion.LookRotation(dir, Vector3.up);
       
        lookDir.x = 0;
        lookDir.z = 0;

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, lookDir, rotSpeed));
    }

    void Shoot()
    {
        GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/EnergyGunSFX"));
        Instantiate(Resources.Load("Prefabs/ProjectileBoss"), weapon.transform.position, weapon.transform.rotation);
    }

    void Punch()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < ccAttackRange + .25f)
        {
            if (!player.GetComponent<PlayerCombat>().shieldActive)
            {
                player.GetComponent<PlayerHealth>().health -= bossCCAttackDamage;
                player.GetComponent<Animator>().SetTrigger("GotHit");
                var gotHitEffect = GameObject.Find("LightningPlayer").GetComponent<ParticleSystem>();
                gotHitEffect.Play();
            }
        }
    }

    void CancelWeaponCD()
    {
        weaponCD = false;
    }

    void EndBSA1()
    {
        bsaAttack = false;
    }

    void BSAAttack()
    {
        var explosionFX = transform.Find("EnergyExplosion").GetComponent<ParticleSystem>();
        explosionFX.Play();

        Collider[] hitPlayer = Physics.OverlapSphere(animator.transform.position, bsaAttackrange + 2);

        foreach (var player in hitPlayer)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<PlayerHealth>().health -= bossBSAAtkDamage;
                player.GetComponent<Animator>().SetTrigger("GotHit");
                var gotHitEffect = GameObject.Find("LightningPlayer").GetComponent<ParticleSystem>();
                gotHitEffect.Play();
            }
        }
    }
}
