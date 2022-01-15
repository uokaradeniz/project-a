using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    public float projectileSpeed;
    Rigidbody rb;
    Transform gun;
    Vector3 firstPos;
    public float projectileRange;
    public ParticleSystem dissapearFX;
    ParticleSystem tookHitEffect;

    public int damageMax;
    public int damageMin;
    int damage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        firstPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        damage = Random.Range(damageMin, damageMax);

        float distance = Vector3.Distance(firstPos, transform.position);
        rb.velocity = transform.forward * projectileSpeed;

        if (distance > projectileRange)
        {
            if(!dissapearFX.isPlaying)
                dissapearFX.Play();

            GameObject.Destroy(this.gameObject, 0.04f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (gameObject.CompareTag("ProjectileEnemy"))
        {
            if (other.CompareTag("Player") && !other.GetComponent<PlayerHealth>().playerIsDead)
            {
                if (!other.GetComponent<PlayerCombat>().shieldActive)
                {
                    other.GetComponent<PlayerHealth>().health -= damage;
                    other.GetComponent<Animator>().SetTrigger("GotHit");
                    var gotHitEffect = GameObject.Find("LightningPlayer").GetComponent<ParticleSystem>();
                    gotHitEffect.Play();
                    dissapearFX.Play();
                    GameObject.Destroy(this.gameObject, 0.04f);
                } else
                {
                    dissapearFX.Play();
                    GameObject.Destroy(this.gameObject, 0.04f);
                }
            }

        }

        if (!other.CompareTag("Projectile") && !other.CompareTag("Player") && !other.CompareTag("Enemy") && !other.CompareTag("ProjectileEnemy") && !other.CompareTag("MapBorder"))
        {
            dissapearFX.Play();
            GameObject.Destroy(this.gameObject, 0.04f);
        }
    }
}
