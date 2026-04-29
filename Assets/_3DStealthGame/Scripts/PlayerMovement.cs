using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction Sprint;

    Animator m_Animator;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    public bool isSprinting;
    public bool isRecovering;

    public int stamina = 50;
    public Slider staminaBar;

    private bool isSprintingCoroutineRunning = false;
    private Coroutine sprintCoroutine;
    private Coroutine recoveryCoroutine;
    private float regenTimer;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();

        m_Rigidbody = GetComponent<Rigidbody> ();
        MoveAction.Enable();
        Sprint.Enable();

        staminaBar.maxValue = 50;
        staminaBar.value = stamina;
    }



    void FixedUpdate ()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();
        
        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);
        
        m_Rigidbody.MoveRotation (m_Rotation);
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);

    }
    //sprinting

    void Update()
    {

        isSprinting = Sprint.IsPressed();
        if(isSprinting == true && isRecovering == false)
        {
            ManageSpeedState(2);
            if(sprintCoroutine == null)
            {
                sprintCoroutine = StartCoroutine(Sprinting());
            }
        }
        else if(isSprinting == false && isRecovering == false)
        {
            ManageSpeedState(1);
            if(sprintCoroutine != null)
            {
                StopCoroutine(sprintCoroutine);
                sprintCoroutine = null;
            }
        }
        else if (isRecovering)
        {
            ManageSpeedState(3);
        }

        if(isSprinting == false && isRecovering == false && stamina < 50)
        {
            regenTimer += Time.deltaTime;

            if(regenTimer >= 0.1f)
            {
                stamina += 1;
                staminaBar.value = stamina;
                regenTimer = 0f;
            }
        }
        else
        {
            regenTimer = 0f;
        }
    }
IEnumerator Sprinting()
    {
        while(stamina > 0 && isSprinting)
        {
            stamina -= 1;
            staminaBar.value = stamina;
            yield return new WaitForSeconds(0.05f);
        }
        sprintCoroutine = null;
        if(recoveryCoroutine == null)
        recoveryCoroutine = StartCoroutine(Recovery());
    }

IEnumerator Recovery()
    {
        isRecovering = true;
        while(stamina < 50)
        {
            stamina += 1;
            staminaBar.value = stamina;
            yield return new WaitForSeconds(0.1f);
        }
        isRecovering = false;
        recoveryCoroutine = null;
    }

public void ManageSpeedState(int speedState)
    {
        switch (speedState)
        {
            case 1: 
            //Default
            walkSpeed = 2;
            staminaBar.fillRect.GetComponent<Image>().color = Color.white;
            break;

            case 2:
            //Sprinting
            walkSpeed = 3;
            staminaBar.fillRect.GetComponent<Image>().color = Color.yellow;
            break;

            case 3:
            //Recovery
            walkSpeed = 1;
            staminaBar.fillRect.GetComponent<Image>().color = Color.red;
            break;

            

        }
    }

}


