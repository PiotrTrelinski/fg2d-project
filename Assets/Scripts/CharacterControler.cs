using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockType
{
    Standing,
    Crouching,
    Either
}
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
    public bool isInStance = false;
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
    public bool isInHitStun = false;
    public bool isInBlockStun = false;
    public bool outgoingAttackLanded = false;
    public int consecutiveHits = 0;
    //player attributes
    private float maxHealth = 100;
    public float currentHealth;
    public float outputDamage;
    public float outputHitStun;
    public float outputBlockStun;
    public float outputPushBack;
    public BlockType outputBlockType;
    private float inputPushBack;
    public bool isKOd = false;
    private Dictionary<string, AttackPropertiesStructure> attackProperties;
    //coliders
    public Collider standingCollider;
    public Collider crouchingCollider;
    //helpers
    private string playerNumberSufix = " P";
    public bool crossFadingAttack = false;
    public float aerialVelX = 0;

    public Rigidbody rb; 

    //attack properties stucture
    public struct AttackPropertiesStructure
    {
        public float damage;
        public float hitStun;
        public float blockStun;
        public float pushBack;
        public BlockType blockType;
        public AttackPropertiesStructure(float damage, float hitStun, float blockStun, float pushBack, BlockType blockType)
        {
            this.damage = damage;
            this.hitStun = hitStun;
            this.blockStun = blockStun;
            this.pushBack = pushBack;
            this.blockType = blockType;
        }
    }

    void Start () {
        speed = walkSpeed;
        transform.Find("stickmanV2").Find("Cube").GetComponent<Renderer>().material.color = playerColor;
        playerNumberSufix += playerNumber;
        currentHealth = maxHealth;
        SetUpAttackProperties();
    }
    void SetUpAttackProperties()
    {
        attackProperties = new Dictionary<string, AttackPropertiesStructure>();
        attackProperties.Add("StandingLeftPunch", new AttackPropertiesStructure(10, 30, 25, 3, BlockType.Standing));
        attackProperties.Add("StandingRightPunch", new AttackPropertiesStructure(15, 27, 24, 2, BlockType.Standing));
        attackProperties.Add("StandingLeftKick", new AttackPropertiesStructure(14, 24, 15, 1, BlockType.Crouching));
        attackProperties.Add("StandingRightKick", new AttackPropertiesStructure(12, 37, 20, 1, BlockType.Standing));
        attackProperties.Add("CrouchingLeftPunch", new AttackPropertiesStructure(6, 12, 17, 1, BlockType.Either));
        attackProperties.Add("CrouchingRightPunch", new AttackPropertiesStructure(18, 40, 23, 2, BlockType.Standing));
        attackProperties.Add("CrouchingLeftKick", new AttackPropertiesStructure(14, 55, 17, 0, BlockType.Crouching));
        attackProperties.Add("CrouchingRightKick", new AttackPropertiesStructure(13, 28, 28, 3, BlockType.Standing));
        attackProperties.Add("DashingLeftPunch", new AttackPropertiesStructure(10, 18, 22, 1, BlockType.Standing));
        attackProperties.Add("DashingRightPunch", new AttackPropertiesStructure(12, 22, 17, 3, BlockType.Standing));
        attackProperties.Add("DashingLeftKick", new AttackPropertiesStructure(8, 12, 12, 0, BlockType.Crouching));
        attackProperties.Add("DashingRightKick", new AttackPropertiesStructure(20, 40, 50, 1, BlockType.Standing));
        attackProperties.Add("JumpingLeftPunch", new AttackPropertiesStructure(16, 40, 10, 3, BlockType.Standing));
        attackProperties.Add("JumpingRightPunch", new AttackPropertiesStructure(12, 22, 23, 5, BlockType.Standing));
        attackProperties.Add("JumpingLeftKick", new AttackPropertiesStructure(25, 60, 10, 3, BlockType.Standing));
        attackProperties.Add("JumpingRightKick", new AttackPropertiesStructure(15, 40, 19, 3, BlockType.Standing));
    }

    // Update is called once per frame
    void Update() {
        if (!isKOd)
        {
            if (currentHealth <= 0)
            {
                HandleKO();
            }
            else
            {
                GetStanceButton();
                if (!isInHitStun)
                {
                    consecutiveHits = 0;
                    GatherCombatInputs();
                    if (!isInBlockStun) {
                        HandleMovement();
                        HandleCombat();
                    }
                    else
                        HandlePushBack(); 
                    HandleAnimation();
                }
                else
                {
                    HandlePushBack();
                }
            }
        }
        else
        {
            HandleGeneralCollider();
        }
    }
    private void HandleKO()
    {
        isKOd = true;
        if (hitFromFront)
            animator.CrossFade("KnockOutFront", 0.3f);
        else
            animator.CrossFade("KnockOutBack", 0.3f);
        animator.SetBool("isKOd", isKOd);
    }
    private void HandlePushBack()
    {
        if (isInHitStun) activeFrames = false;
        grounded = IsGrounded();
        if (/*grounded && */currentHealth > 0)
        {
                if (facingLeft)
                {
                    if (hitFromFront)
                        rb.velocity = new Vector3(inputPushBack, rb.velocity.y, 0);
                    else
                        rb.velocity = new Vector3(-inputPushBack, rb.velocity.y, 0);
                }
                else
                {
                    if (hitFromFront)
                        rb.velocity = new Vector3(-inputPushBack, rb.velocity.y, 0);
                    else
                        rb.velocity = new Vector3(inputPushBack, rb.velocity.y, 0);
                }
            
        }
        
    }
    private void GetStanceButton()
    {
        if (Input.GetButtonDown("Stance Trigger" + playerNumberSufix) && !isInBlockStun)
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
        //GatherCombatInputs();
        if (isCancelable)
        {
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
                                SetOutputDamage(attackProperties["StandingLeftPunch"]);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                animator.Play("CombatStandingRightPunch");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(attackProperties["StandingRightPunch"]);
                            }
                            else if (firstInput == "Left Kick")
                            {
                                animator.Play("CombatStandingLeftKick");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(attackProperties["StandingLeftKick"]);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                animator.Play("CombatStandingRightKick");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputDamage(attackProperties["StandingRightKick"]);
                            }
                        }
                        else
                        {
                            StartAttack();
                            if (firstInput == "Left Punch")
                            {
                                animator.Play("CombatCrouchingLeftPunch");
                                SetOutputDamage(attackProperties["CrouchingLeftPunch"]);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                crossFadingAttack = true;
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightPunch", 0.3f);
                                SetOutputDamage(attackProperties["CrouchingRightPunch"]);
                            }
                            else if (firstInput == "Left Kick")
                            {
                                animator.Play("CombatCrouchingLeftKick");
                                SetOutputDamage(attackProperties["CrouchingLeftKick"]);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                crossFadingAttack = true;
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightKick", 0.3f);
                                SetOutputDamage(attackProperties["CrouchingRightKick"]);
                            }
                        }
                    }
                }
                
                else if (isDashingForward && rb.velocity.y > 0)
                {
                    StartAttack();
                    if (firstInput == "Left Punch")
                    {
                        animator.Play("CombatDashingLeftPunch");
                        SetOutputDamage(attackProperties["DashingLeftPunch"]);
                    }
                    else if (firstInput == "Right Punch")
                    {
                        animator.Play("CombatDashingRightPunch");
                        SetOutputDamage(attackProperties["DashingRightPunch"]);
                    }
                    else if (firstInput == "Left Kick")
                    {
                        animator.Play("CombatDashingLeftKick");
                        SetOutputDamage(attackProperties["DashingLeftKick"]);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        animator.CrossFade("CombatDashingRightKick", 0.3f);
                        SetOutputDamage(attackProperties["DashingRightKick"]);
                    }
                }
                else if(!isDashing)
                {
                    StartAttack();
                    aerialVelX = rb.velocity.x;
                    if (firstInput == "Left Punch")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingLeftPunch");
                        SetOutputDamage(attackProperties["JumpingLeftPunch"]);
                    }
                    else if (firstInput == "Right Punch")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingRightPunch");
                        SetOutputDamage(attackProperties["JumpingRightPunch"]);
                    }
                    else if (firstInput == "Left Kick")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingLeftKick");
                        SetOutputDamage(attackProperties["JumpingLeftKick"]);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingRightKick");
                        SetOutputDamage(attackProperties["JumpingRightKick"]);
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
            animator.Play("CombatBadLanding");
            activeFrames = false;
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
        outgoingAttackLanded = false;
    }

    private void HandleAnimation()
    {
        animator.SetFloat("vertSpeed", rb.velocity.y);
        animator.SetBool("grounded", grounded);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInStance", isInStance);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isInHitStun", isInHitStun);
        animator.SetBool("isInBlockStun", isInBlockStun);

        animator.SetFloat("speed", Math.Abs(Input.GetAxis("Horizontal" + playerNumberSufix)));
        
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
                isDashing = false;
                isDashingForward = false;
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
        LimitVelocity();
        HandleGeneralCollider();
    }

    private void LimitVelocity()
    {
        if (rb.velocity.y > 20)
        {
            rb.velocity = new Vector3(rb.velocity.x, 20, 0);
        }
        if (rb.velocity.x > 15)
        {
            rb.velocity = new Vector3(15, rb.velocity.y, 0);
        }
        if (rb.velocity.x < -15)
        {
            rb.velocity = new Vector3(-15, rb.velocity.y, 0);
        }
    }

    public void HandleGeneralCollider()
    {
        if (isKOd)
        {
            standingCollider.enabled = false;
            crouchingCollider.enabled = false;
        }
        else if (isCrouching)
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
        if (((Input.GetAxisRaw("Horizontal" + playerNumberSufix) < 0 && facingLeftDash) 
            ^ (Input.GetAxisRaw("Horizontal" + playerNumberSufix) > 0 && !facingLeftDash)) 
            && Time.time - lastTime < 0.15f)
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

    public void InvocationOfVulnerability()
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
        isInHitStun = false;
        isInBlockStun = false;
        isInStance = false;
        isAerialAttacking = false;
        isCancelable = true;
        isAttacking = false;
        isDashing = false;
        isCrouching = false;
        isDashing = false;
        isDashingForward = false;
        isRunning = false;
        crossFadingAttack = false;
        activeFrames = false;
        isKOd = false;
        outgoingAttackLanded = false;

        HandleGeneralCollider();
        animator.SetBool("isKOd", isKOd);
        animator.Play("NeutralIdle");
    }
    private void SetOutputDamage(AttackPropertiesStructure attackProperty)
    {
        outputDamage = attackProperty.damage;
        outputHitStun = attackProperty.hitStun;
        outputBlockStun = attackProperty.blockStun;
        outputPushBack = attackProperty.pushBack;
        outputBlockType = attackProperty.blockType;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopAerialInterference(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        StopAerialInterference(collision);
    }

    public bool CheckBlockCondition(BlockType block)
    {
        return grounded && hitFromFront && isInStance && !isInHitStun && !isAttacking
               && ((block == BlockType.Standing && !isCrouching)
               ^ (block == BlockType.Crouching && isCrouching)
               || block == BlockType.Either)
               && ((facingLeft && Input.GetAxis("Horizontal" + playerNumberSufix) >= 0)
               ^ (!facingLeft && Input.GetAxis("Horizontal" + playerNumberSufix) <= 0));
    }

    internal void ApplyBlockStun(float inputBlockStun, string hitZone, float pushBack)
    {
        invulnerable = true;
        InvocationOfVulnerability();
        isInBlockStun = true;
        inputPushBack = pushBack;
        activeFrames = false;

        animator.SetFloat("blockStun", (60 / inputBlockStun));
        Debug.Log("blockstun:" + ((60 / inputBlockStun) + " inframes:" + (60 / ((60 / inputBlockStun)))));
        animator.SetBool("isInBlockStun", isInBlockStun);
        string animationToPlay = "";
        if (hitZone == "Head" || hitZone == "UpperSpine" || hitZone == "Arm_R" || hitZone == "Arm_L")
        {
            if (!isCrouching)
                animationToPlay = "BlockStandingHigh";
            else
                animationToPlay = "BlockCrouchingHigh";
        }
        else
        {
            if (!isCrouching)
                animationToPlay = "BlockStandingMid";
            else
                animationToPlay = "BlockCrouchingMid";
        }
        animator.Play(animationToPlay);
    }

    internal void ApplyHitStun(float inputHitStun, string hitZone, float pushBack, float damage)
    {
        invulnerable = true;
        InvocationOfVulnerability();
        isInHitStun = true;
        crossFadingAttack = false;
        inputPushBack = pushBack;

        currentHealth -= damage;

        Debug.Log("hit:"+consecutiveHits);
        animator.SetFloat("hitStun", (60/inputHitStun)+(0.7f*consecutiveHits));
        Debug.Log("hitstun:" + ((60 / inputHitStun) + (0.7f * consecutiveHits)) + " inframes:" + (60/((60 / inputHitStun) + (0.7f * consecutiveHits))));
        consecutiveHits += 1;
        animator.SetBool("isInHitStun", isInHitStun);
        string animationToPlay = "";
        if (hitZone == "Head" || hitZone == "UpperSpine" || hitZone == "Arm_R"|| hitZone == "Arm_L")
        {
            if (hitFromFront)
            {
                if (!isCrouching)
                    animationToPlay = "HitReactionStandingHighFront";
                else
                    animationToPlay = "HitReactionCrouchingHighFront";
            }
            else
            {
                if (!isCrouching)
                    animationToPlay = "HitReactionStandingHighBack";
                else
                    animationToPlay = "HitReactionCrouchingHighBack";
            }
        }
        else if(hitZone == "LowerSpine" || hitZone == "UpperLeg_L" || hitZone == "UpperLeg_R")
        {
            if (hitFromFront)
            {
                if (!isCrouching)
                    animationToPlay = "HitReactionStandingMidFront";
                else
                    animationToPlay = "HitReactionCrouchingMidFront";
            }
            else
            {
                if (!isCrouching)
                    animationToPlay = "HitReactionStandingMidBack";
                else
                    animationToPlay = "HitReactionCrouchingMidBack";
            }
        }
        else if(hitZone == "LowerLeg_L")
        {
            animationToPlay = "HitReactionStandingLowL";
        }
        else if (hitZone == "LowerLeg_R")
        {
            animationToPlay = "HitReactionStandingLowR";
        }
        animator.Play(animationToPlay);
    }


    private void StopAerialInterference(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && collision.gameObject != this.gameObject)
        {
            CharacterControler otherChar = collision.gameObject.GetComponent<CharacterControler>();
            if (!grounded && !isDashing)
            {
                if (otherChar.isAttacking && ((otherChar.facingLeft && rb.velocity.x < 0) || (!otherChar.facingLeft && rb.velocity.x > 0)))
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                }
            }
            if (isAttacking && isAerialAttacking)
            {
                if (aerialVelX > 0 && otherChar.rb.velocity.x < 0 || aerialVelX < 0 && otherChar.rb.velocity.x > 0)
                {
                    aerialVelX = 0;
                }
            }
        }
    }
}
