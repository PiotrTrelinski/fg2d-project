using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControler : MonoBehaviour
{
    public Animator animator;
    public string playerNumber = "1";
    public Color playerColor = Color.black;
    //horizontal movement
    public float walkSpeed = 5;
    public float runSpeed = 15;
    public float speed = 0;
    private float lastTime = 0;
    private bool facingLeftDash = false;
    private bool isRunning = false;
    private float lastFrameHorizontalAxis;
    public bool facingLeft = false;
    //vertical movement
    public float jumpForce = 20;
    public bool grounded = false;
    private bool lastFrameGrounded;
    public Transform[] groundedChecks;
    private Collider[] collisionsWithGround;
    public LayerMask groundLayer;
    //stance
    private bool isInStance = false;
    public bool isCrouching = false;
    private bool lastFrameCrouching = false;
    private bool lastFrameStance = false;
    public float dashVertForce = 5;
    public float dashHorForce = 15;
    public float stanceJumpForce = 10;
    private bool isDashingForward = false;
    private bool isDashing = false;
    //combat
    public bool isAttacking = false;
    public bool isCancelable = true;
    public bool isAerialAttacking = false;
    private string firstInput = null;
    private string secondInput = null;
    private float lastCombatInputTime;
    private float inputLeniency = 0.4f;
    public bool activeFrames = false;
    public bool invulnerable = false;
    public string activeLimb;
    public bool hitFromFront;
    //player attributes
    private float maxHealth = 100;
    public float currentHealth;
    public float outputDamage;
    public bool isKOd = false;
    //coliders
    public Collider standingCollider;
    public Collider crouchingCollider;
    public Collider proneCollider;
    //helpers
    private string playerNumberSufix = " P";

    public Rigidbody rb; 

    void Start () {
        speed = walkSpeed;
        transform.Find("stickmanV2").Find("Cube").GetComponent<Renderer>().material.color = playerColor;
        playerNumberSufix += playerNumber;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update() {
        //    Debug.Log(Input.GetAxis("Horizontal"));
        //    Debug.Log((animator.GetCurrentAnimatorStateInfo(0).IsName("NeutralIdle") || animator.GetCurrentAnimatorStateInfo(0).IsName("StanceIdle")));
        //PlayerPrefs.DeleteAll();
        if (!isKOd)
        {
            HandleMovement();
            HandleCombat();
            HandleAnimation();
        }
        else
        {
            HandleGeneralCollider();
        }
    }
    private void GatherCombatInputs()
    {
        if(firstInput == null)
        {
            if (Input.GetButtonDown("Left Punch" + playerNumberSufix))
            {
                firstInput = "Left Punch";
            }
            else if (Input.GetButtonDown("Right Punch" + playerNumberSufix))
            {
                firstInput = "Right Punch";
            }
            else if (Input.GetButtonDown("Left Kick" + playerNumberSufix))
            {
                firstInput = "Left Kick";
            }
            else if (Input.GetButtonDown("Right Kick" + playerNumberSufix))
            {
                firstInput = "Right Kick";
            }
            if (firstInput != null)
                lastCombatInputTime = Time.time;
            //if(firstInput!=null) Debug.Log("firstInput:" + firstInput);
        }
        if(firstInput!=null && secondInput == null && Time.time - lastCombatInputTime <= 0.05f)
        {
            if (Input.GetButtonDown("Left Punch" + playerNumberSufix))
            {
                if(firstInput!= "Left Punch")
                    secondInput = "Left Punch";
            }
            if (Input.GetButtonDown("Right Punch" + playerNumberSufix))
            {
                if (firstInput != "Right Punch")
                    secondInput = "Right Punch";
            }
            if (Input.GetButtonDown("Left Kick" + playerNumberSufix))
            {
                if (firstInput != "Left Kick")
                    secondInput = "Left Kick";
            }
            if (Input.GetButtonDown("Right Kick" + playerNumberSufix))
            {
                if (firstInput != "Right Kick")
                    secondInput = "Right Kick";
            }
            //Debug.Log("secondInput:" + secondInput);
        }
        if(firstInput != null && Time.time - lastCombatInputTime >= inputLeniency)
        {
            firstInput = null;
            secondInput = null;
        }
    }

    private void HandleCombat()
    {
        GatherCombatInputs();
        if (isCancelable)
        {
            if (Input.GetButtonDown("Stance Trigger" + playerNumberSufix))
            {
                if (isInStance)
                {
                    isInStance = false;
                }

                else if (!isInStance)
                {
                    isRunning = false;
                    isInStance = true;
                }
            }
            if (firstInput != null && Time.time - lastCombatInputTime >= 0.05f)
            {
                activeLimb = firstInput;
                
                if (grounded)
                {

                    if ((firstInput == "Left Punch" && secondInput == "Right Kick") || (firstInput == "Right Kick" && secondInput == "Left Punch"))
                    {
                        Debug.Log("rzut do przodu");
                    }
                    else if ((firstInput == "Right Punch" && secondInput == "Left Kick") || (firstInput == "Left Kick" && secondInput == "Right Punch"))
                    {
                        Debug.Log("rzut do tyłu");
                    }
                    else
                    {
                        if (!isCrouching)
                        {
                            StartAttack();
                            if (firstInput == "Left Punch")
                            {
                                animator.Play("CombatStandingLeftPunch");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(10);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                animator.Play("CombatStandingRightPunch");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(15);
                            }
                            else if (firstInput == "Left Kick")
                            {
                                animator.Play("CombatStandingLeftKick");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(14);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                animator.Play("CombatStandingRightKick");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(12);
                            }
                        }
                        else
                        {
                            StartAttack();
                            if (firstInput == "Left Punch")
                            {
                                animator.Play("CombatCrouchingLeftPunch");
                                SetOutputDamage(6);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightPunch", 0.3f);
                                SetOutputDamage(12);
                            }
                            else if (firstInput == "Left Kick")
                            { 
                                animator.Play("CombatCrouchingLeftKick");
                                SetOutputDamage(14);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightKick", 0.3f);
                                SetOutputDamage(20);
                            }
                        }
                    }
                }
                else if (isDashingForward && rb.velocity.y > 0)
                {
                    StartAttack();
                    if (firstInput == "Right Punch")
                    {
                        animator.Play("CombatDashingRightPunch");
                        SetOutputDamage(12);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        animator.CrossFade("CombatDashingRightKick", 0.3f);
                        SetOutputDamage(20);
                    }
                    else if (firstInput == "Left Punch")
                    {
                        animator.Play("CombatDashingLeftPunch");
                        SetOutputDamage(10);
                    }
                    else if (firstInput == "Left Kick")
                    {
                        animator.Play("CombatDashingLeftKick");
                        SetOutputDamage(8);
                    }
                }
                else if(!isDashing)
                {
                    StartAttack();
                    if (firstInput == "Right Punch")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingRightPunch");
                        SetOutputDamage(12);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingRightKick");
                        SetOutputDamage(15);
                    }
                    else if (firstInput == "Left Punch")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingLeftPunch");
                        SetOutputDamage(16);
                    }
                    else if (firstInput == "Left Kick")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingLeftKick");
                        SetOutputDamage(25);
                    }
                }
                firstInput = null;
                secondInput = null;
            }
        }
        if (isAerialAttacking && grounded && !isCancelable)
        {
            rb.velocity = Vector3.zero;
            isAerialAttacking = false;
            isCrouching = true;
            activeFrames = false;
            animator.CrossFade("CombatBadLanding", 0.1f);
        }
        if(grounded && !lastFrameGrounded && isDashingForward && isAttacking)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void StartAttack()
    {
        isCancelable = false;
        isAttacking = true;
        
    }

    private void HandleAnimation()
    {
        animator.SetFloat("vertSpeed", rb.velocity.y);
        animator.SetBool("grounded", grounded);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInStance", isInStance);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isAttacking", isAttacking);
        
        animator.SetFloat("speed", Math.Abs(Input.GetAxis("Horizontal" + playerNumberSufix)));
        
        if(currentHealth <= 0)
        {
            isKOd = true;
            if(hitFromFront)
                animator.CrossFade("KnockOutFront", 0.3f);
            else
                animator.CrossFade("KnockOutBack", 0.3f);
            animator.SetBool("isKOd", isKOd);
        }
        
        if (!isAttacking)
        {
            //Neutral jump/land/crouch/neutral transfer
            if (!isInStance)
            {
                if (grounded && !lastFrameGrounded && !isInStance)
                    animator.CrossFade("NeutralIdle", 0.1f);
                if (!grounded && lastFrameGrounded && !isInStance)
                    animator.CrossFade("NeutralFloatTree", 0.1f);
                if (lastFrameStance)
                    if (grounded)
                        if (isCrouching && lastFrameCrouching)
                            animator.CrossFade("NeutralCrouchIdle", 0.2f);
                        else
                            animator.CrossFade("NeutralIdle", 0.1f);
                if (isCrouching && !lastFrameCrouching)
                    animator.CrossFade("NeutralCrouchIdle", 0.2f);
            }
            //Stance jump/land/crouch/stance transfer
            if (isInStance)
            {
                if (!grounded && lastFrameGrounded)
                    animator.CrossFade("StanceFloatTree", 0.1f);
                if (grounded && !lastFrameGrounded)
                    animator.CrossFade("StanceIdle", 0.1f);
                if (!facingLeft)
                    animator.SetFloat("horSpeed", rb.velocity.x);
                else
                    animator.SetFloat("horSpeed", -rb.velocity.x);
                if (!lastFrameStance)
                    if (grounded)
                        if (isCrouching && lastFrameCrouching)
                            animator.CrossFade("StanceCrouchIdle", 0.1f);
                        else
                            animator.CrossFade("StanceIdle", 0.1f);
                if (isCrouching && !lastFrameCrouching)
                    animator.CrossFade("StanceCrouchIdle", 0.1f);
            }
        }

        lastFrameGrounded = grounded;
        lastFrameCrouching = isCrouching;
        lastFrameStance = isInStance;
    }

    private void HandleMovement()
    {
        grounded = IsGrounded();
        if (!isAttacking)
        {
            
            if (!isInStance)
            {

                if (grounded && Input.GetAxis("Horizontal" + playerNumberSufix) < 0)
                {
                    facingLeft = true;
                    
                }
                else if (grounded && Input.GetAxis("Horizontal" + playerNumberSufix) > 0)
                {
                    facingLeft = false;                
                }
                if (facingLeft)
                {
                    transform.eulerAngles = new Vector3(0, 270, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 90, 0);
                }

            }
            if (grounded && !isCrouching && !isInStance) rb.velocity = new Vector3(Input.GetAxis("Horizontal" + playerNumberSufix) * speed, rb.velocity.y, 0);
            else if (grounded && !isCrouching && isInStance) rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal" + playerNumberSufix) * speed, rb.velocity.y, 0);
            else if (grounded && isCrouching)
            {
                isRunning = false;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            if (grounded && Input.GetAxis("Vertical" + playerNumberSufix) > 0)
            {
                grounded = false;
                if (!isInStance) rb.velocity = (new Vector3(rb.velocity.x, jumpForce, 0));
                else rb.velocity = (new Vector3(rb.velocity.x, stanceJumpForce, 0));
            }
            if (grounded && Input.GetAxisRaw("Vertical" + playerNumberSufix) < 0)
                isCrouching = true;
            else
                isCrouching = false;
            if (grounded)
            {
                isDashingForward = false;
                isDashing = false;
            }
            HandleDoubleTapDash();
        }
        HandleGeneralCollider();
    }

    public void HandleGeneralCollider()
    {
        if (isKOd)
        {
            standingCollider.enabled = false;
            crouchingCollider.enabled = false;
            proneCollider.enabled = true;
        }
        else if (isCrouching)
        {
            crouchingCollider.enabled = true;
            standingCollider.enabled = false;
            proneCollider.enabled = false;
        }
        else
        {
            crouchingCollider.enabled = false;
            standingCollider.enabled = true;
            proneCollider.enabled = false;
        }
    }

    private void HandleDoubleTapDash()
    {
        if (((Input.GetAxisRaw("Horizontal" + playerNumberSufix) < 0 && facingLeftDash) ^ (Input.GetAxisRaw("Horizontal" + playerNumberSufix) > 0 && !facingLeftDash)) && Time.time - lastTime < 0.15f)
        {
            if(!isInStance)
                isRunning = true;
            if (isInStance && grounded)
            {
                isDashing = true;
                rb.velocity = (new Vector3(Input.GetAxisRaw("Horizontal" + playerNumberSufix) * dashHorForce, dashVertForce, 0));
                if((facingLeft && Input.GetAxisRaw("Horizontal" + playerNumberSufix) < 0) ||(!facingLeft && Input.GetAxisRaw("Horizontal" + playerNumberSufix) > 0))
                {
                    isDashingForward = true;
                }
            }
        }

        if (isRunning && Input.GetAxis("Horizontal" + playerNumberSufix) != 0)
            speed = runSpeed;
        else
        {
            speed = walkSpeed;
            isRunning = false;
        }
        if (Input.GetAxisRaw("Horizontal" + playerNumberSufix) == 0 && lastFrameHorizontalAxis != 0)
        {
            lastTime = Time.time;
            if (lastFrameHorizontalAxis > 0) facingLeftDash = false;
            else if (lastFrameHorizontalAxis < 0) facingLeftDash = true;
        }

        if (Input.GetAxisRaw("Horizontal" + playerNumberSufix) != 0)
            lastFrameHorizontalAxis = Input.GetAxis("Horizontal" + playerNumberSufix);
        else
            lastFrameHorizontalAxis = 0;
    }

    private bool IsGrounded()
    {
        foreach (var groundedCheck in groundedChecks)
        {
            collisionsWithGround = Physics.OverlapSphere(groundedCheck.position, 0.1f, groundLayer);
            if (collisionsWithGround.Length > 0) return true;
        }
        return false;
    }

    public void InvocationOfInvulnerability()
    {
        Invoke("SwitchOffVulnerability", 0.2f);
    }
    private void SwitchOffVulnerability()
    {
        invulnerable = false;
    }
    internal void RefillHealth()
    {
        currentHealth = maxHealth;
    }
    internal void ResetToNeutral()
    {
        isInStance = false;
        isAerialAttacking = false;
        isCancelable = true;
        isAttacking = false;
        isDashing = false;
        isCrouching = false;
        isDashing = false;
        isDashingForward = false;
        isRunning = false;
        activeFrames = false;
        isKOd = false;
        HandleGeneralCollider();
        animator.Play("NeutralIdle");
    }
    private void SetOutputDamage(float damage)
    {
        outputDamage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopAerialInterference(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        StopAerialInterference(collision);
    }
    private void StopAerialInterference(Collision collision)
    {
        if (!grounded && !isDashing && collision.gameObject.tag == "Player" && collision.gameObject != this.gameObject)
        {
            CharacterControler otherChar = collision.gameObject.GetComponent<CharacterControler>();
            if (otherChar.isAttacking && ((otherChar.facingLeft && rb.velocity.x < 0) || (!otherChar.facingLeft && rb.velocity.x > 0)))
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
    }
}
