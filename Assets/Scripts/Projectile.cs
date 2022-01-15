using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed;
    Rigidbody rb;
    Transform gun;
    Vector3 firstPos;
    public float projectileRange;
    public ParticleSystem dissapearFX;
    PlayerCombat playerCombat;

    public bool isExplosive;
    bool lostPhysics;

    float explosiveTimer;
    public float explodeTime;
    public float destroyExplosive;
    bool expFXPlayed;

    // Start is called before the first frame update
    void Start()
    {
        playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody>();
        transform.position = playerCombat.gun.transform.position;
        firstPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isExplosive)
        {
            explosiveTimer += Time.fixedDeltaTime;

            Collider[] enemiesExp = Physics.OverlapSphere(transform.position, playerCombat.explosionRange);

            foreach (var hitEnemy in enemiesExp)
            {
                if (explosiveTimer >= explodeTime)
                {
                    if (!expFXPlayed)
                    {
                        GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/ExplosiveSFX"));
                        dissapearFX.Play();
                        expFXPlayed = true;
                    }
                    if (hitEnemy.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit - ExplosiveAttack");
                        if (hitEnemy.GetComponent<EnemyAI>().enemyType != EnemyAI.EnemyType.EnemySpawner)
                            hitEnemy.transform.Find("LightningEnemy").GetComponent<ParticleSystem>().Play();

                        if (hitEnemy.GetComponent<EnemyAI>().enemyType != EnemyAI.EnemyType.Turret && hitEnemy.GetComponent<EnemyAI>().enemyType != EnemyAI.EnemyType.EnemySpawner)
                        {
                            hitEnemy.GetComponent<EnemyAI>().animator.SetTrigger("GotHit");
                        }
                        hitEnemy.GetComponent<EnemyAI>().enemyHealth -= playerCombat.sa1Damage;
                    }

                    if (hitEnemy.CompareTag("Boss"))
                    {
                        Debug.Log("Hit - ExplosiveAttack");
                        hitEnemy.GetComponent<BossAI>().bossHealth -= playerCombat.sa1Damage;
                        hitEnemy.GetComponent<Animator>().SetTrigger("GotHit");
                        hitEnemy.transform.Find("LightningEnemy").GetComponent<ParticleSystem>().Play();
                    }
                }
            }
        }

        if (!isExplosive)
        {
            float distance = Vector3.Distance(firstPos, transform.position);
            rb.velocity = transform.forward * projectileSpeed;

            if (distance > projectileRange)
            {
                if (!dissapearFX.isPlaying)
                    dissapearFX.Play();

                GameObject.Destroy(this.gameObject, 0.04f);
            }
        }
        else
        {
            if (playerCombat.gameBehaviour.sa1Activate && !lostPhysics)
            {
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.Acceleration);
                lostPhysics = true;
            }
            GameObject.Destroy(this.gameObject, destroyExplosive * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isExplosive)
        {
            if (other.CompareTag("Enemy"))
            {
                playerCombat.gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/HitIndicatorSFX"));
                dissapearFX.Play();
                EnemyAI enemyAI = other.GetComponent<EnemyAI>();
                enemyAI.enemyHealth -= playerCombat.gunDamage;
                if (enemyAI.enemyType != EnemyAI.EnemyType.EnemySpawner)
                {
                    if (other.transform.Find("LightningEnemy").GetComponent<ParticleSystem>() != null)
                    {
                        other.transform.Find("LightningEnemy").GetComponent<ParticleSystem>().Play();
                    }
                    if (enemyAI.enemyType != EnemyAI.EnemyType.Turret)
                        enemyAI.animator.SetTrigger("GotHit");
                }
                GameObject.Destroy(this.gameObject, 0.04f);
                Debug.Log("Hit - ProjectileAttack");

            } else if(other.CompareTag("Boss"))
            {
                playerCombat.gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/HitIndicatorSFX"));
                other.GetComponent<BossAI>().bossHealth -= playerCombat.gunDamage;
                other.transform.Find("LightningEnemy").GetComponent<ParticleSystem>().Play();
                GameObject.Destroy(this.gameObject, 0.04f);
                Debug.Log("Hit - ProjectileAttack");
            }
        }

        if (!isExplosive)
        {
            if (!other.CompareTag("Enemy") && !other.CompareTag("Player") && !other.CompareTag("Projectile") && !other.CompareTag("ProjectileEnemy") && !other.CompareTag("Ore") && !other.CompareTag("MapBorder") && !other.CompareTag("IgnoreProjectile"))
            {
                dissapearFX.Play();
                GameObject.Destroy(this.gameObject, 0.04f);
            }
        }
    }
}
