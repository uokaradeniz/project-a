using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [HideInInspector]public GameBehaviour gameBehaviour;
    GameObject projectile;
    [HideInInspector]public Transform gun;
    Animator animator;

    public bool projectileAttack;
    public bool closeAttack;

    ParticleSystem muzzleFlash;
    ParticleSystem swingEffect;
    ParticleSystem shieldEffect;

    public int gunDamageMin;
    public int gunDamageMax;
    public float heatValue;
    public float heatCDValue;
    public int ccDamageMin;
    public int ccDamageMax;

    public float shootDuration;
    float shootTimer;
    public bool cannotAttack;
    public float fadeTimer;
    bool fadeAnimBool;

    public int sa1Damage;
    public float explosionRange;
    [HideInInspector] public float sa1Timer;
    public float sa1Duration;

    //Shield
    public float shieldDuration;
    float shieldTimer;
    float shieldCooldown;
    private float shieldCooldownSpeed = 1;
    public bool shieldActive;
    bool shieldEffectPlaying;
    bool shieldUsable = true;
    TextMeshProUGUI shieldCounter;

    public int oreDamage;

    bool diBugFix;
    int comboTickCount;
    bool allowC2;
    public float comboTimer;

    [HideInInspector]public int gunDamage;
    [HideInInspector]public int closeCombatDamage;

    // Start is called before the first frame update
    void Start()
    {
        gameBehaviour = GameObject.Find("SystemSettings").GetComponent<GameBehaviour>();
        animator = GetComponent<Animator>();
        projectile = (GameObject)Resources.Load("Prefabs/Projectile");
        gun = transform.Find("Gun");
        shieldCounter = GameObject.Find("ShieldCounter").GetComponent<TextMeshProUGUI>();
        muzzleFlash = transform.Find("PlayerModel/Arm1/Pipe/Cube/MuzzleFlash").GetComponent<ParticleSystem>();
        swingEffect = transform.Find("PlayerModel/Arm1/Pipe/Cube/Cube/ParticleSwing/SwingEffect").GetComponent<ParticleSystem>();
        shieldEffect = transform.Find("PlayerModel/Cone/EnergyShield").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        gunDamage = Random.Range(gunDamageMin, gunDamageMax);
        closeCombatDamage = Random.Range(ccDamageMin, ccDamageMax);
        shootTimer += shootDuration * Time.deltaTime;
        sa1Timer -= sa1Duration * Time.deltaTime;

        if(gameBehaviour.projectileSlider.value > 0)
        gameBehaviour.projectileSlider.value -= heatCDValue;

        if (gameBehaviour.projectileSlider.value >= .9f)
        {
            cannotAttack = true;
            gameBehaviour.gunCDRText.enabled = true;
        }

        if (cannotAttack && gameBehaviour.projectileSlider.value <= 0)
        {
            cannotAttack = false;
            gameBehaviour.gunCDRText.enabled = false;
        }

        if (Input.GetButtonDown("SpecialAttack1") && gameBehaviour.score >= gameBehaviour.SA1NeededScore && sa1Timer <= 0 && !gameBehaviour.gameStopped)
        {
            sa1Timer = 3;
            gameBehaviour.sa1Warn.enabled = true;
            Invoke("CloseUsingSkillWarn", .5f);
            gameBehaviour.score -= gameBehaviour.SA1NeededScore;
            gameBehaviour.sa1Activate = true;
            Invoke("EndSA1", 1);
            gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/ThrowSFX"));
            Instantiate(Resources.Load("Prefabs/Explosive"),gun.transform.position,gun.transform.rotation);
        }


        int shieldCooldownConv = Mathf.RoundToInt(shieldCooldown);

        if (shieldCooldown >= 10.4)
            shieldCounter.text = "Ready";
        else if (shieldCooldown < 10.399)
            shieldCounter.text = shieldCooldownConv.ToString();

        if(gameBehaviour.damageIncrease && !diBugFix)
        {
            diBugFix = true;
            ccDamageMax = ccDamageMax + 15;
            ccDamageMin = ccDamageMin + 15;
            gunDamageMax = gunDamageMax + 15;
            gunDamageMin = gunDamageMin + 15;
            Invoke("CloseDIPowerup", 10);
        }

        if (Input.GetButtonDown("ActivateShield") && shieldUsable && !gameBehaviour.gameStopped)
        {
            shieldActive = true;
            gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/ShieldSFX"));
            shieldUsable = false;
        }

        if (shieldActive)
        {
            if (!shieldEffectPlaying)
                shieldEffect.Play();

            shieldEffectPlaying = true;
            shieldTimer += shieldDuration * Time.deltaTime;
        }

        if(shieldTimer >= 3)
        {
            shieldActive = false;
            shieldEffect.Stop();
            shieldEffectPlaying = false;
            shieldTimer = 0;
        }

        if (!shieldUsable)
            shieldCooldown -= shieldCooldownSpeed * Time.deltaTime;

        if (shieldCooldown <= 0.5f)
        {
            shieldUsable = true;
            shieldCooldown = 10.41f;
        }

        if (fadeAnimBool)
            fadeTimer += Time.deltaTime;

        if (fadeTimer >= 3)
        {
            gameBehaviour.projectileSlider.GetComponent<Animator>().SetBool("FadeIn",false);
            fadeTimer = 0;
            fadeAnimBool = false;
        }

        if (shootTimer > 2)
        {
            if (Input.GetButton("Fire2") && !cannotAttack && !gameBehaviour.gameStopped)
            {
                projectileAttack = true;
                shootTimer = 0;
            }
        }

        if (projectileAttack)
        {
            fadeTimer = 0;
            if (fadeTimer < 3)
                gameBehaviour.projectileSlider.GetComponent<Animator>().SetBool("FadeIn", true);
            fadeAnimBool = true;
            closeAttack = false;
            comboTickCount = 0;
            allowC2 = false;
        }

        if (Input.GetButtonDown("Fire1") && !closeAttack && !gameBehaviour.gameStopped)
        {
            closeAttack = true;
            comboTickCount++;
        }

        if (comboTickCount > 2)
            comboTickCount = 2;

        if (comboTickCount == 1)
            comboTimer += Time.fixedDeltaTime * 2f;

        if (comboTimer > 2)
            StoppedCloseAttack();

        if (closeAttack)
           projectileAttack = false;

        if(allowC2 && Input.GetButtonDown("Fire1") && !gameBehaviour.gameStopped)
            comboTickCount++;

        animator.SetBool("ProjectileAttack", projectileAttack);
        animator.SetBool("CloseAttack", closeAttack);
        animator.SetInteger("ComboAnim", comboTickCount);
    }

    void CloseDIPowerup()
    {
        ccDamageMax = ccDamageMax - 15;
        ccDamageMin = ccDamageMin - 15;
        gunDamageMax = gunDamageMax - 15;
        gunDamageMin = gunDamageMin - 15;
        diBugFix = false;
        gameBehaviour.damageIncrease = false;
        gameBehaviour.pudDI.enabled = false;
    }

    void CloseUsingSkillWarn()
    {
        gameBehaviour.sa1Warn.enabled = false;
    }

    void Shoot()
    {
        gameBehaviour.projectileSlider.value += heatValue;
        Instantiate(projectile, gun.transform.position, gun.transform.rotation);
        gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/EnergyGunSFX"));
        muzzleFlash.Play();
    }

    void StoppedProjectileAttack()
    {
        projectileAttack = false;
    }

    void StoppedCloseAttack()
    {
        comboTickCount = 0;
        closeAttack = false;
        allowC2 = false;
        swingEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        GetComponentInChildren<CloseCombat>().tookHit = false;
        comboTimer = 0;
    }

    void PlaySwingEffect()
    {
        gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/EnergySwordSwingSFX"));
        if(comboTickCount == 1)
        swingEffect.Play();
    }

    void EndSA1()
    {
        gameBehaviour.sa1Activate = false;
    }

    void AllowC2()
    {
        allowC2 = true;
    }

    void ResetHitState()
    {
        GetComponentInChildren<CloseCombat>().tookHit = false;
    }
}
