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
    //player attributes
    private float maxHealth = 100;
    public float currentHealth;
    public float outputDamage;
    //coliders
    public Collider standingCollider;
    public Collider crouchingCollider;
    //helpers
    private string playerNumberSufix = " P";
    public int times = 0;
    public Text text;

    public Rigidbody rb;
    private bool isDashing;
    

    void Start () {
        speed = walkSpeed;
        transform.Find("stickmanV2").Find("Cube").GetComponent<Renderer>().material.color = playerColor;
        playerNumberSufix += playerNumber;
        currentHealth = maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
        //    Debug.Log(Input.GetAxis("Horizontal"));
        //    Debug.Log((animator.GetCurrentAnimatorStateInfo(0).IsName("NeutralIdle") || animator.GetCurrentAnimatorStateInfo(0).IsName("StanceIdle")));
        //PlayerPrefs.DeleteAll();
        HandleMovement();
        HandleCombat();
        HandleAnimation();
        if(playerNumber == "2")text.text = "" + times;
        text.text += " hp = " + currentHealth;
    }
    private void GatherCombatInputs()
    {
        if(firstInput == null)
        {
            if (Input.GetButtonDown("Left Punch" + playerNumberSufix))
            {
                firstInput = "Left Punch";
                outputDamage = 5;
            }
            else if (Input.GetButtonDown("Right Punch" + playerNumberSufix))
            {
                firstInput = "Right Punch";
                outputDamage = 10;
            }
            else if (Input.GetButtonDown("Left Kick" + playerNumberSufix))
            {
                firstInput = "Left Kick";
                outputDamage = 15;
            }
            else if (Input.GetButtonDown("Right Kick" + playerNumberSufix))
            {
                firstInput = "Right Kick";
                outputDamage = 20;
            }
            if (firstInput != null) lastCombatInputTime = Time.time;
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
                    //if (secondInput != null)
                    //{
                    //    //Debug.Log(firstInput);
                    //    //Debug.Log(secondInput);
                    //    //Debug.Log((firstInput == "Left Punch" && secondInput == "Right Kick")|| (firstInput == "Right Kick" && secondInput == "Left Punch"));
                    //    //Debug.Log((firstInput == "Right Punch" && secondInput == "Left Kick") || (firstInput == "Left Kick" && secondInput == "Right Punch"));
                    //    if ((firstInput == "Left Punch" && secondInput == "Right Kick")|| (firstInput == "Right Kick" && secondInput == "Left Punch"))
                    //    {
                    //        Debug.Log("rzut do przodu");
                    //    }
                    //    if ((firstInput == "Right Punch" && secondInput == "Left Kick") || (firstInput == "Left Kick" && secondInput == "Right Punch"))
                    //    {
                    //        Debug.Log("rzut do tyłu");
                    //    }
                    //}
                    //else

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
                                animator.CrossFade("CombatStandingLeftPunch", 0.1f);
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                animator.CrossFade("CombatStandingRightPunch", 0.1f);
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                            }
                            else if (firstInput == "Left Kick")
                            {
                                animator.CrossFade("CombatStandingLeftKick", 0.1f);
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                animator.CrossFade("CombatStandingRightKick", 0.1f);
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                            }
                        }
                        else
                        {
                            StartAttack();
                            if (firstInput == "Left Punch")
                            {
                                animator.CrossFade("CombatCrouchingLeftPunch", 0.1f);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightPunch", 0.1f);
                            }
                            else if (firstInput == "Left Kick")
                            { 
                                animator.CrossFade("CombatCrouchingLeftKick", 0.1f);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightKick", 0.1f);
                            }
                        }
                    }
                }
                else if (isDashingForward && rb.velocity.y > 0)
                {
                    StartAttack();
                    if (firstInput == "Right Punch")
                    {
                        animator.CrossFade("CombatDashingRightPunch", 0.1f);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        animator.CrossFade("CombatDashingRightKick", 0.1f);
                    }
                    else if (firstInput == "Left Punch")
                    {
                        animator.CrossFade("CombatDashingLeftPunch", 0.1f);
                    }
                    else if (firstInput == "Left Kick")
                    {

                        animator.CrossFade("CombatDashingLeftKick", 0.1f);
                    }
                }
                else if(!isDashing)
                {
                    StartAttack();
                    if (firstInput == "Right Punch")
                    {
                        isAerialAttacking = true;
                        animator.CrossFade("CombatJumpingRightPunch", 0.1f);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        isAerialAttacking = true;
                        animator.CrossFade("CombatJumpingRightKick", 0.1f);
                    }
                    else if (firstInput == "Left Punch")
                    {
                        isAerialAttacking = true;
                        animator.CrossFade("CombatJumpingLeftPunch", 0.1f);
                    }
                    else if (firstInput == "Left Kick")
                    {
                        isAerialAttacking = true;
                        animator.CrossFade("CombatJumpingLeftKick", 0.1f);
                    }
                }
                firstInput = null;
                secondInput = null;
            }
        }
        if (isAerialAttacking && grounded)
        {
            animator.CrossFade("StanceIdle", 0.1f);
            isAerialAttacking = false;
            isAttacking = false;
            isCancelable = true;
        }

        
    }

    private void StartAttack()
    {
        isAttacking = true;
        isCancelable = false;
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
        //Neutral jump/land/crouch/neutral transfer
        if (!isAttacking)
        {
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
                    transform.eulerAngles = new Vector3(0, 270, 0);
                }
                else if (grounded && Input.GetAxis("Horizontal" + playerNumberSufix) > 0)
                {
                    facingLeft = false;
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
        if (isCrouching)
        {
            crouchingCollider.enabled = true;
            standingCollider.enabled = false;
        }
        else
        {
            crouchingCollider.enabled = false;
            standingCollider.enabled = true;
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
        Invoke("SwitchOffVulnerability", 0.1f);
    }
    private void SwitchOffVulnerability()
    {
        invulnerable = false;
    }
}
