using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mining : MonoBehaviour
{
    public int orePoints;
    public bool canMine;

    public bool botCanMine;

    // Update is called once per frame
    void Update()
    {
        if (orePoints <= 0)
        {
            GameObject.Find("SystemSettings").GetComponent<GameBehaviour>().oreDepleted = true;
            GameObject.Destroy(this.gameObject, 0.2f);
             
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MineBot"))
        {
            botCanMine = true;
            other.GetComponent<Animator>().SetBool("CanMine", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("MineBot"))
        {
            botCanMine = false;
            other.GetComponent<Animator>().SetBool("CanMine", false);
        }
    }
}
