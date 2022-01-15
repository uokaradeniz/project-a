using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    EnemyAI enemyAI;
    public float spawnSpeed;
    float spawnTimer;
    public Transform spawnPoint;
    public bool canSpawn;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("Player").GetComponent<PlayerHealth>().playerIsDead)
        {
            if (canSpawn)
            {
                if (!GameObject.Find("Player").GetComponent<PlayerHealth>().playerIsDead)
                {
                    if (enemyAI.enemyType == EnemyAI.EnemyType.EnemySpawner)
                    {
                        spawnTimer += spawnSpeed * Time.deltaTime;
                        if (spawnTimer >= 5)
                        {
                            Invoke("CreateSnitch", 1);
                            spawnTimer = 0;
                        }
                    }
                }
            }
        }
    }

    void CreateSnitch()
    {
        Instantiate(Resources.Load("Prefabs/Characters/SnitchEnemy"), spawnPoint.position, transform.rotation, transform.parent);
    }
}
