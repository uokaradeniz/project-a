using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombat : MonoBehaviour
{
    PlayerCombat playerCombat;
    [HideInInspector]public bool tookHit;
    ParticleSystem tookHitEffect;

    private void Start()
    {
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerCombat.closeAttack)
        {
            if (!tookHit)
            {
                if (other.CompareTag("Enemy"))
                {
                    playerCombat.gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/HitIndicatorSFX"));

                    Debug.Log("Hit - CloseAttack");
                    other.GetComponent<EnemyAI>().enemyHealth -= playerCombat.closeCombatDamage;
                    if (other.GetComponent<EnemyAI>().enemyType != EnemyAI.EnemyType.EnemySpawner)
                    {
                        if (other.GetComponent<EnemyAI>().enemyType != EnemyAI.EnemyType.Turret)
                            other.GetComponent<Animator>().SetTrigger("GotHit");
                        tookHitEffect = other.transform.Find("LightningEnemy").GetComponent<ParticleSystem>();
                        tookHitEffect.Play();
                    }
                    tookHit = true;
                }

                if(other.CompareTag("Boss"))
                {
                    playerCombat.gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/HitIndicatorSFX"));

                    other.GetComponent<BossAI>().bossHealth -= playerCombat.closeCombatDamage;
                    other.transform.Find("LightningEnemy").GetComponent<ParticleSystem>().Play();
                    tookHit = true;
                }
            }

            if(other.CompareTag("Ore"))
            {
                playerCombat.gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/HitIndicatorSFX"));
                other.GetComponent<Mining>().orePoints -= playerCombat.oreDamage;
                playerCombat.gameBehaviour.score += playerCombat.oreDamage;
            }
        }
    }

}
