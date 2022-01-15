using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public float moveSpeed;
    float smoothRotateEffect = 20;
    Quaternion turn;
    Rigidbody rb;

    public bool dash;
    bool dashing;

    GameBehaviour gameBehaviour;

    PlayerCombat playerCombat;
    Animator animator;

    ParticleSystem dashEffect;
    public int dashCount;
    public float dashPower;
    public float dashTimer;

    bool fmBugFix;

    // Start is called before the first frame update
    void Start()
    {
        dashEffect = transform.Find("DashEffect").GetComponent<ParticleSystem>();
        gameBehaviour = GameObject.Find("SystemSettings").GetComponent<GameBehaviour>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GetComponent<PlayerHealth>().playerIsDead)
        {
            if (dashCount >= 3)
            {
                dashTimer += Time.deltaTime;
                if (dashTimer >= 3)
                {
                    dashCount = 0;
                    dashTimer = 0;
                }
            }

            if (gameBehaviour.fasterMovement && !fmBugFix)
            {
                fmBugFix = true;
                moveSpeed = 0.25f;
                Invoke("CloseFMPowerup", 10);
            }

            if (gameBehaviour.score >= gameBehaviour.SA3NeededScore && dashCount < 3)
            {
                if (Input.GetButtonDown("Dash") && !gameBehaviour.gameStopped)
                {
                    dashCount++;
                    dash = true;
                    Invoke("CloseDash", 0.1f);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!GetComponent<PlayerHealth>().playerIsDead)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            rb.MovePosition(transform.position + move * moveSpeed);

            if (!gameBehaviour.joystickConnected)
            {
                transform.Find("JoystickAim").localScale = new Vector3(0, 0, 0);
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.y - transform.position.y));
                Quaternion targetRot = Quaternion.LookRotation(mousePos - new Vector3(transform.position.x, 0, transform.position.z));
                transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRot.eulerAngles.y, smoothRotateEffect);
                playerCombat.gun.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRot.eulerAngles.y, 0);
            } else
            {
                transform.Find("JoystickAim").localScale = new Vector3(0.5f, 0.5f, 1);
                float joyTargetRot = Mathf.Atan2(Input.GetAxis("Joystick X"), Input.GetAxis("Joystick Y")) * Mathf.Rad2Deg;
                float joyRotSmoothed = Mathf.MoveTowardsAngle(transform.eulerAngles.y, joyTargetRot, smoothRotateEffect * 0.4f);
                if(Input.GetAxis("Joystick X") > 0.01 || Input.GetAxis("Joystick Y") > 0.01 || Input.GetAxis("Joystick X") < -0.01 || Input.GetAxis("Joystick Y") < -0.01)
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, joyRotSmoothed, transform.eulerAngles.z);
            }
            if (dash)
            {
                if (!dashing)
                {
                    gameBehaviour.sa3Warn.enabled = true;
                    dashEffect.Play();
                    playerCombat.gun.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/DashSFX"));
                    gameBehaviour.score -= gameBehaviour.SA3NeededScore;
                    dashing = true;
                    Invoke("CloseUsingSkillWarn", 0.5f);
                }
                rb.AddForce(Input.GetAxis("Horizontal") * dashPower, 0, Input.GetAxis("Vertical") * dashPower, ForceMode.Impulse);
            }
        }
    }

    void CloseDash()
    {
        dashing = false;
        dash = false;
        dashEffect.Stop();
    }

    void CloseUsingSkillWarn()
    {
        gameBehaviour.sa3Warn.enabled = false;
    }

    void CloseFMPowerup()
    {
        moveSpeed = 0.15f;
        gameBehaviour.fasterMovement = false;
        gameBehaviour.pudFM.enabled = false;
        fmBugFix = false;
    }
}
