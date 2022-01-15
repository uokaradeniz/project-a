using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public int enemyHealth;

    [HideInInspector] public Animator animator;

    Rigidbody rb;

    GameObject player;

    NavMeshAgent navMesh;

    ParticleSystem muzzleFlash;

    GameObject projectile;

    public bool enemyDead;

    //Turret
    float shootTimer;
    public float shootDuration;
    public float rotationSpeed;
    public bool sawPlayer;
    public float maxDistanceFromPlayer;
    public float checkDistance;
    Transform enemyGun;
    Transform turretWeapon;

    float wanderTime;
    public float wanderPathDuration;

    public enum EnemyType
    {
        Dummy,
        Turret,
        Snitch,
        Wanderer,
        EnemySpawner
    }

    public EnemyType enemyType;

    private void Start()
    {           
        projectile = (GameObject)Resources.Load("Prefabs/ProjectileTurret");
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        if (enemyType == EnemyType.Turret)
        {
            turretWeapon = transform.Find("EnemyModel/TurretWeapon");
            enemyGun = transform.Find("EnemyModel/TurretWeapon/Cube/Cube/TurretGun");
            muzzleFlash = transform.Find("EnemyModel/TurretWeapon/Cube/Cube/MuzzleFlash").GetComponent<ParticleSystem>();
        }

        if(enemyType == EnemyType.Wanderer)
        {
            enemyGun = transform.Find("EnemyModel/WandererWeapon/Cube/Cube/WandererGun");
            muzzleFlash = transform.Find("EnemyModel/WandererWeapon/Cube/Cube/MuzzleFlash").GetComponent<ParticleSystem>();
        }
    }

    private void Update()
    {
        if (enemyHealth <= 0 && !enemyDead)
        {
            GetComponent<Collider>().enabled = false;

            if (enemyType == EnemyType.Dummy)
            {
                player.GetComponent<PlayerCombat>().gameBehaviour.score += player.GetComponent<PlayerCombat>().gameBehaviour.scoreByDummy;
            }

            if (enemyType == EnemyType.Wanderer)
            {
                player.GetComponent<PlayerCombat>().gameBehaviour.score += player.GetComponent<PlayerCombat>().gameBehaviour.scoreByWanderer;
            }

            if (enemyType == EnemyType.Turret)
            {
                player.GetComponent<PlayerCombat>().gameBehaviour.score += player.GetComponent<PlayerCombat>().gameBehaviour.scoreByTurret;
            }

            if (enemyType == EnemyType.Snitch)
            {
                navMesh.destination = transform.position;
                player.GetComponent<PlayerCombat>().gameBehaviour.score += player.GetComponent<PlayerCombat>().gameBehaviour.scoreBySnitch;
            }
            enemyDead = true;
            GameObject.Destroy(this.gameObject, 0.1f);
        }

        if (enemyType == EnemyType.Snitch)
            SnitchAI();

        if (enemyType == EnemyType.Wanderer)
        {
            WandererAI();
        }

        if (enemyType == EnemyType.Turret)
            TurretAI();
    }

    void SnitchAI()
    {
        if (!player.GetComponent<PlayerHealth>().playerIsDead)
        {
            navMesh.SetDestination(player.transform.position);
            if (navMesh.velocity != Vector3.zero)
                animator.SetBool("Moving", true);
            else
                animator.SetBool("Moving", false);
        }
        else
        {
            navMesh.SetDestination(transform.position);
            animator.SetBool("Moving", false);
        }
    }

    void WandererAI()
    {
        wanderTime += Time.fixedDeltaTime * wanderPathDuration;

        if (navMesh.velocity != Vector3.zero)
            animator.SetBool("Moving", true);
        else
            animator.SetBool("Moving", false);

        float distFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (wanderTime >= 3)
        {
            Vector3 newPos = WandererRandomDirection(transform.position, 15, -1);
            navMesh.SetDestination(newPos);
            wanderTime = 0;
        }

        if (distFromPlayer <= 4.5)
        {
            shootDuration = Random.Range(0.25f, 1);
            navMesh.SetDestination(Vector3.MoveTowards(transform.position, player.transform.position, -navMesh.speed));
        }
        else
            shootDuration = Random.Range(1, 5);

        if (distFromPlayer <= maxDistanceFromPlayer)
        {
            Vector3 desiredDir = player.transform.position - enemyGun.transform.position;

            if (Physics.Raycast(enemyGun.transform.position, desiredDir, out RaycastHit hit, checkDistance))
            {
                if (hit.collider.CompareTag("Player"))
                    sawPlayer = true;
                else
                    sawPlayer = false;
            }

        }
        else
        {
            sawPlayer = false;
        }

        if (!player.GetComponent<PlayerHealth>().playerIsDead)
        {
            if (sawPlayer)
            {
                navMesh.angularSpeed = 0;
                Vector3 targetDirection = player.transform.position - transform.position;
                targetDirection.y = 0;
                Vector3 rot = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed, 0);
                transform.rotation = Quaternion.LookRotation(rot);
                enemyGun.transform.rotation = Quaternion.LookRotation(rot);

                animator.SetBool("WandererAttack", true);
                shootTimer += shootDuration * Time.deltaTime;
            } else
            {
                navMesh.angularSpeed = 120;
                animator.SetBool("WandererAttack", false);
            }
        }
    }


    public Vector3 WandererRandomDirection(Vector3 origin, float distance, int layerMask)
    {   
        Vector3 randomDir = Random.insideUnitSphere * distance;
        randomDir += origin;
        NavMesh.SamplePosition(randomDir, out NavMeshHit navMeshHit, distance, layerMask);
        return navMeshHit.position;
    }

    void TurretAI()
    {
        float distFromPlayer = Vector3.Distance(turretWeapon.transform.position, player.transform.position);;

        if(distFromPlayer <= maxDistanceFromPlayer) 
        {
            Vector3 desiredDir = player.transform.position - turretWeapon.transform.position;

            if(Physics.Raycast(turretWeapon.transform.position,desiredDir,out RaycastHit hit,checkDistance))
            {
                if (hit.collider.CompareTag("Player"))
                    sawPlayer = true;
                else
                    sawPlayer = false;
            }

        } else
        {
            sawPlayer = false;
        }

        if (!player.GetComponent<PlayerHealth>().playerIsDead)
        {
            if (sawPlayer)
            {
                Vector3 targetDirection = player.transform.position - turretWeapon.transform.position;
                targetDirection.y = 0;
                Vector3 rot = Vector3.RotateTowards(turretWeapon.transform.forward, targetDirection, rotationSpeed, 0);
                turretWeapon.transform.rotation = Quaternion.LookRotation(rot);

                shootTimer += shootDuration * Time.deltaTime;
                if (shootTimer >= 3)
                {
                    enemyGun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/EnergyGunSFX"));
                    Instantiate(projectile, enemyGun.transform.position, turretWeapon.transform.rotation);
                    muzzleFlash.Play();
                    shootTimer = 0;
                }
            }
        }
    }

    public void WandererAttack()
    {
        if (shootTimer >= 1)
        {
            enemyGun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/EnergyGunSFX"));
            Instantiate(projectile, enemyGun.transform.position, enemyGun.transform.rotation);
            muzzleFlash.Play();
            shootTimer = 0;
        }
    }
}

