using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialEvents : MonoBehaviour
{
    bool activateDoor;
    public float doorSpeed;

    Vector3 oldPos;
    Vector3 newPos;
    AudioSource bossMusic;

    private void Start()
    {
        if(gameObject.name == "ActivateBossRoom")
        {
           bossMusic = GameObject.Find("SystemSettings").GetComponent<AudioSource>();
        }

        if (gameObject.name == "ActivateDoor")
        {
            newPos = new Vector3(transform.Find("Door").position.x, 5.01f, transform.Find("Door").position.z);
            oldPos = transform.Find("Door").position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == "ActivateHUD-Combat")
        {
            if (other.CompareTag("Player"))
            {
                GameObject.Find("HUD").GetComponent<Animator>().SetTrigger("Animate");
                other.GetComponent<PlayerCombat>().enabled = true;
                other.GetComponent<PlayerLocomotion>().moveSpeed = 0.15f;
                DestroyThisObject();
            }
        }

        if (gameObject.name == "ActivateBossRoom")
        {
            if (bossMusic.isPlaying)
            {
                bossMusic.Stop();
                bossMusic.PlayOneShot((AudioClip)Resources.Load("Sounds/BossMusic"));
            } else
                bossMusic.PlayOneShot((AudioClip)Resources.Load("Sounds/BossMusic"));

            GameObject.Find("BossMecha").GetComponent<BossAI>().bossAIActive = true;
            GameObject.Find("EnemySpawner (1)").GetComponent<EnemyRespawner>().canSpawn = true;
            GameObject.Find("EnemySpawner (2)").GetComponent<EnemyRespawner>().canSpawn = true;
            DestroyThisObject();
        }

        if (gameObject.name == "ActivateEnemySpawner")
        {
            if (other.CompareTag("Player"))
            {
                GameObject.Find("EnemySpawner").GetComponent<EnemyRespawner>().canSpawn = true;
                DestroyThisObject();
            }
        }

        if (gameObject.name == "ActivateDoor")
        {
            if (other.CompareTag("Player")) {
                activateDoor = true;
                GetComponent<AudioSource>().Play();
            }
        }

        if(gameObject.name == "ActivateMining")
        {
            if(other.CompareTag("Player"))
            {
                GameObject.Find("SystemSettings").GetComponent<GameBehaviour>().miningActivated = true; ;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.name == "ActivateDoor")
        {
            if (other.CompareTag("Player"))
            {
                activateDoor = false;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.name == "ActivateDoor")
        {
            if (activateDoor)
            {
                transform.Find("Door").position = Vector3.MoveTowards(transform.Find("Door").position, newPos, Time.deltaTime * doorSpeed);
            }
            else
            {
                transform.Find("Door").position = Vector3.MoveTowards(transform.Find("Door").position, oldPos, Time.deltaTime * doorSpeed);
            }
        }
    }

    void DestroyThisObject()
    {
        Destroy(this.gameObject, .5f);
    }
}
