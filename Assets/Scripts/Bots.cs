using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bots : MonoBehaviour
{
    public bool isMineBot;
    NavMeshAgent navMesh;

    public int botOreDamage;

    Animator animator;

    GameBehaviour gameBehaviour;

    [HideInInspector]public bool destroyBot;

    public float botLifeTimer;
    public float botLifeDuration;

    ParticleSystem mineParticle;

    // Start is called before the first frame update
    void Start()
    {
        mineParticle = transform.Find("Body/Pickaxe/Cone/SparksEffect").GetComponent<ParticleSystem>();
        gameBehaviour = GameObject.Find("SystemSettings").GetComponent<GameBehaviour>();
        animator = GetComponent<Animator>();

        if (isMineBot)
            navMesh = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        botLifeTimer += botLifeDuration * Time.deltaTime;

        if (botLifeTimer >= Random.Range(3, 6))
            destroyBot = true;

        if (destroyBot) {
            gameBehaviour.mineBotList.Remove(this.gameObject);
            Destroy(this.gameObject, Random.Range(2, 10));
        }

        if (navMesh.velocity != Vector3.zero)
            animator.SetBool("Moving", true);
        else
            animator.SetBool("Moving", false);

        if (isMineBot && gameBehaviour.ore != null)
        {
            navMesh.SetDestination(gameBehaviour.ore.transform.position);
        }
    }

    void Mine()
    {
        if (gameBehaviour.ore != null)
        {
            if (gameBehaviour.ore.GetComponent<Mining>().botCanMine)
            {
                gameBehaviour.ore.GetComponent<Mining>().orePoints -= botOreDamage;
                mineParticle.Play();
                GameObject.Find("SystemSettings").GetComponent<GameBehaviour>().score += botOreDamage;
            }
        }
    }
}