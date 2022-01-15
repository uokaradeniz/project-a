using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{

    public enum PowerupType
    {
        FasterMovement,
        DamageIncrease
    }

    public PowerupType powerupType;
    GameBehaviour gameBehaviour;

    GameObject player;

    bool stackOnplayer;

    // Start is called before the first frame update
    void Start()
    {
        gameBehaviour = GameObject.Find("SystemSettings").GetComponent<GameBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.gameObject;
            player.GetComponent<PlayerCombat>().gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/PowerupSFX"));
            if (powerupType == PowerupType.FasterMovement && !gameBehaviour.fasterMovement)
            {
                stackOnplayer = true;

                gameBehaviour.fasterMovement = true;
                gameBehaviour.pudFM.enabled = true;
            }

            if (powerupType == PowerupType.DamageIncrease && !gameBehaviour.damageIncrease)
            {
                stackOnplayer = true;

                gameBehaviour.damageIncrease = true;
                gameBehaviour.pudDI.enabled = true;
            }
        }
    }

    private void Update()
    {
        if (stackOnplayer)
        {
            transform.position = player.transform.position;
            GameObject.Destroy(this.gameObject, 10.2f);
        }
    }
}