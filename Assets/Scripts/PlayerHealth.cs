using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    TextMeshProUGUI errorMessage;
    TextMeshProUGUI healthIndicator;
    Image emBacklight;
    public bool playerIsDead;
    ParticleSystem pdEnergyField;
    public bool godMode;

    // Start is called before the first frame update
    void Start()
    {
        healthIndicator = GameObject.Find("HealthIndicator").GetComponent<TextMeshProUGUI>();
        errorMessage = GameObject.Find("ErrorMessage").GetComponent<TextMeshProUGUI>();
        emBacklight = GameObject.Find("EMBacklight").GetComponent<Image>();
        pdEnergyField = GameObject.Find("PDEnergyField").GetComponent<ParticleSystem>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (godMode)
            health = 100;

        healthIndicator.text = health.ToString();

        if (health <= 0)
            healthIndicator.text = "Null";
        else if (health >= 100)
            health = 100;

        if (health <= 30 && health > 15)
        {
            Camera.main.GetComponent<GlitchEffect>().enabled = true;
            Camera.main.GetComponent<GlitchEffect>().flipIntensity = 0.25f;
            Camera.main.GetComponent<GlitchEffect>().colorIntensity = 0.4f;
        }
        else if (health <= 15)
        {
            Camera.main.GetComponent<GlitchEffect>().colorIntensity = 0.75f;
            Camera.main.GetComponent<GlitchEffect>().flipIntensity = 1;
        }
        else if (health > 30)
            Camera.main.GetComponent<GlitchEffect>().enabled = false;

        if (health <= 0 && !playerIsDead)
        {
            playerIsDead = true;
            Camera.main.GetComponent<GlitchEffect>().enabled = true;
            Camera.main.GetComponent<GlitchEffect>().colorIntensity = 0.75f;
            Camera.main.GetComponent<GlitchEffect>().flipIntensity = 1;
            GetComponent<PlayerCombat>().gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/PlayerDeathSFX"));
            GetComponent<PlayerCombat>().enabled = false;
            GetComponent<Animator>().SetTrigger("PlayerDead");
            GameObject.Find("RespawnMessage").transform.localScale = new Vector3(1,1,1);
            pdEnergyField.Play();
            Invoke("DisableAnimator", 1f);
            Invoke("EnableErrorMessage", 0.1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Misc") && health < 100)
        {
            health += Random.Range(5, 25);
            GetComponent<PlayerCombat>().gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/HealthPickupSFX"));
            Destroy(collision.collider.gameObject);
        }
    }

    void EnableErrorMessage()
    {
        errorMessage.enabled = true;
        emBacklight.enabled = true;
        Invoke("DisableErrorMessage", 1);
    }

    void DisableErrorMessage()
    {
        errorMessage.enabled = false;
        emBacklight.enabled = false;
        Invoke("EnableErrorMessage", 1);
    }

    void DisableAnimator()
    {
        GetComponent<Animator>().enabled = false;
        GameObject.Find("PlayerModel").SetActive(false);
    }
}
