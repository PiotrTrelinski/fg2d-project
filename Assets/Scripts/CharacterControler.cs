using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControler : MonoBehaviour {
    public Animator animator;
    public string  playerNumber = " P1";
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
    private bool isCrouching = false;
    private bool lastFrameCrouching = false;
    private bool lastFrameStance = false;
    public bool isAttacking = false;
    public float dashVertForce = 5;
    public float dashHorForce = 15;
    public float stanceJumpForce = 10;
    private float dashCooldown;
    //coliders
    public Collider standingCollider;
    public Collider crouchingCollider;


    public Rigidbody rb;

    void Start () {
        speed = walkSpeed;
        transform.Find("stickman").Find("Cube").GetComponent<Renderer>().material.color = playerColor;
    }
	
	// Update is called once per frame
	void Update () {
        //    Debug.Log(Input.GetAxis("Horizontal"));
        //    Debug.Log((animator.GetCurrentAnimatorStateInfo(0).IsName("NeutralIdle") || animator.GetCurrentAnimatorStateInfo(0).IsName("StanceIdle")));
        //PlayerPrefs.DeleteAll();
        HandleMovement();
        HandleCombat();
        HandleAnimation();
    }

    private void HandleCombat()
    {
        if (!isAttacking) {
            if (Input.GetButtonDown("Stance Trigger" + playerNumber))
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
            if (grounded)
            {
                

                if (!isCrouching)
                {
                    if (Input.GetButtonDown("Left Punch" + playerNumber))
                    {
                        isAttacking = true;
                        animator.CrossFade("CombatStandingLeftPunch", 0.1f);
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
                    if (Input.GetButtonDown("Right Punch" + playerNumber))
                    {
                        isAttacking = true;
                        animator.CrossFade("CombatStandingRightPunch", 0.1f);
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
                    if (Input.GetButtonDown("Left Kick" + playerNumber))
                    {
                        isAttacking = true;
                        animator.CrossFade("CombatStandingLeftKick", 0.1f);
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
                    if (Input.GetButtonDown("Right Kick" + playerNumber))
                    {
                        isAttacking = true;
                        animator.CrossFade("CombatStandingRightKick", 0.1f);
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Left Punch" + playerNumber))
                    {
                        isAttacking = true;
                        isCrouching = false;
                        animator.CrossFade("CombatCrouchingLeftPunch", 0.1f);
                    }
                    if (Input.GetButtonDown("Right Punch" + playerNumber))
                    {
                        isAttacking = true;
                        animator.CrossFade("CombatCrouchingRightPunch", 0.1f);
                    }
                    if (Input.GetButtonDown("Left Kick" + playerNumber))
                    {
                        isAttacking = true;
                        isCrouching = false;
                        animator.CrossFade("CombatCrouchingLeftKick", 0.1f);
                    }
                    if (Input.GetButtonDown("Right Kick" + playerNumber))
                    {
                        isAttacking = true;
                        animator.CrossFade("CombatCrouchingRightKick", 0.1f);
                    }
                }
            }
        }

        
    }

    private void HandleAnimation()
    {
        animator.SetFloat("vertSpeed", rb.velocity.y);
        animator.SetBool("grounded", grounded);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInStance", isInStance);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetFloat("speed", Math.Abs(Input.GetAxis("Horizontal" + playerNumber)));
        //Neutral jump/land/crouch/neutral transfer
        if (!isInStance)
        {
            if (grounded && !lastFrameGrounded && !isInStance)
                animator.CrossFade("NeutralIdle", 0.1f);
            if (!grounded && lastFrameGrounded && !isInStance)
                animator.CrossFade("NeutralFloatTree", 0.1f);
            if(lastFrameStance)
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
        if (grounded && !lastFrameGrounded)
        {
            dashCooldown = Time.time;
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

                if (grounded && Input.GetAxis("Horizontal" + playerNumber) < 0)
                {
                    facingLeft = true;
                    transform.eulerAngles = new Vector3(0, 270, 0);
                }
                else if (grounded && Input.GetAxis("Horizontal" + playerNumber) > 0)
                {
                    facingLeft = false;
                    transform.eulerAngles = new Vector3(0, 90, 0);
                }
            }
            if (grounded && !isCrouching && !isInStance) rb.velocity = new Vector3(Input.GetAxis("Horizontal" + playerNumber) * speed, rb.velocity.y, 0);
            else if (grounded && !isCrouching && isInStance) rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal" + playerNumber) * speed, rb.velocity.y, 0);
            else if (grounded && isCrouching)
            {
                isRunning = false;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            if (grounded && Input.GetAxis("Vertical" + playerNumber) > 0)
            {
                grounded = false;
                if (!isInStance) rb.velocity = (new Vector3(rb.velocity.x, jumpForce, 0));
                else rb.velocity = (new Vector3(rb.velocity.x, stanceJumpForce, 0));
            }
            if (grounded && Input.GetAxisRaw("Vertical" + playerNumber) < 0)
                isCrouching = true;
            else
                isCrouching = false;

            HandleDoubleTapDash();
        }
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
        if (((Input.GetAxisRaw("Horizontal" + playerNumber) < 0 && facingLeftDash) ^ (Input.GetAxisRaw("Horizontal" + playerNumber) > 0 && !facingLeftDash)) && Time.time - lastTime < 0.15f)
        {
            if(!isInStance)
                isRunning = true;
            if (isInStance && grounded)
            {
                rb.velocity = (new Vector3(Input.GetAxisRaw("Horizontal" + playerNumber) * dashHorForce, dashVertForce, 0));
            }
        }

        if (isRunning && Input.GetAxis("Horizontal" + playerNumber) != 0)
            speed = runSpeed;
        else
        {
            speed = walkSpeed;
            isRunning = false;
        }
        if (Input.GetAxisRaw("Horizontal" + playerNumber) == 0 && lastFrameHorizontalAxis != 0)
        {
            lastTime = Time.time;
            if (lastFrameHorizontalAxis > 0) facingLeftDash = false;
            else if (lastFrameHorizontalAxis < 0) facingLeftDash = true;
        }

        if (Input.GetAxisRaw("Horizontal" + playerNumber) != 0)
            lastFrameHorizontalAxis = Input.GetAxis("Horizontal" + playerNumber);
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

    private void EndAttackingState()
    {
        isAttacking = false;
    }
}
