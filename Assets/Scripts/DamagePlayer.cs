using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    int damage;
    public int damageMax;
    public int damageMin;

    public float timeTickSpeed;
    float timer;
    EnemyAI enemyAI;

    Collider playerCollider;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    private void OnTriggerStay(Collider other)
    {
        damage = Random.Range(damageMin, damageMax);

        if (!this.gameObject.CompareTag("DamagePlatform") && !this.gameObject.CompareTag("ProjectileEnemy") && !this.gameObject.CompareTag("Projectile"))
        {
            if (enemyAI.enemyType != EnemyAI.EnemyType.Snitch && enemyAI.enemyType != EnemyAI.EnemyType.Wanderer && this.gameObject.CompareTag("DamagePlatform") && !other.CompareTag("Boss"))
            {
                if (other.CompareTag("Player") && !other.GetComponent<PlayerHealth>().playerIsDead)
                {
                    playerCollider = other;
                    timer += timeTickSpeed * Time.deltaTime;
                    if (timer >= Random.Range(2, 5))
                    {
                        other.GetComponent<PlayerHealth>().health -= damage;
                        other.GetComponent<Animator>().SetTrigger("GotHit");
                        var gotHitEffect = GameObject.Find("LightningPlayer").GetComponent<ParticleSystem>();
                        gotHitEffect.Play();

                        timer = 0;
                    }
                }
            }
            else
            {
                if (other.CompareTag("Player") && !other.GetComponent<PlayerHealth>().playerIsDead)
                {
                    playerCollider = other;
                    enemyAI.animator.SetBool("SnitchAttack", true);
                }
            }
        } else
        {
            if (other.CompareTag("Player") && !other.GetComponent<PlayerHealth>().playerIsDead)
            {
                playerCollider = other;
                timer += timeTickSpeed * Time.deltaTime;
                if (timer >= Random.Range(2, 5))
                {
                    other.GetComponent<PlayerHealth>().health -= damage;
                    other.GetComponent<Animator>().SetTrigger("GotHit");
                    var gotHitEffect = GameObject.Find("LightningPlayer").GetComponent<ParticleSystem>();
                    gotHitEffect.Play();

                    timer = 0;
                }
            }
        }
    }

    void SnitchAttack()
    {
        if (!playerCollider.GetComponent<PlayerCombat>().shieldActive)
        {
            playerCollider.GetComponent<PlayerHealth>().health -= damage;
            playerCollider.GetComponent<Animator>().SetTrigger("GotHit");
            var gotHitEffect = GameObject.Find("LightningPlayer").GetComponent<ParticleSystem>();
            gotHitEffect.Play();
        }
    }
    
    void StopSnitchAttack()
    {
        enemyAI.animator.SetBool("SnitchAttack", false);
    }
}
