using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterControler : MonoBehaviour
{
    public Animator animator;
    public int playerNumber = 1;
    public Color playerColor = Color.black;
    public Renderer playerRenderer;
    //horizontal movement
    public float walkSpeed = 5;
    public float runSpeed = 15;
    public float speed = 0;
    public float animationMovementSpeed;
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
    private bool airDashExpanded = false;
    public bool isAirDashing = false;
    public bool isStuckToWall = false;
    //stance
    public bool isInStance = false;
    public bool isCrouching = false;
    private bool lastFrameCrouching = false;
    private bool lastFrameStance = false;
    public float dashVertForce = 5;
    public float dashHorForce = 15;
    public float stanceJumpForce = 10;
    public bool isDashingForward = false;
    public bool isDashing = false;
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
    public string activeLimb = "";
    public bool hitFromFront;
    public bool isInHitStun = false;
    public bool isInBlockStun = false;
    public bool isInThrow = false;
    public bool throwBreakable = false;
    public bool outgoingAttackLanded = false;
    public int consecutiveHits = 0;
    public float comboDamage = 0;
    //player attributes
    public float maxHealth = 200;
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
    public Collider wallInteractionCollider;
    //inputs
    private float horizontalAxis = 0;
    private float verticalAxis = 0;
    private bool leftPunch = false;
    private bool rightPunch = false;
    private bool leftKick = false;
    private bool rightKick = false;
    private bool stanceTrigger = false;
    //helpers
    private int invert = 1;
    private string playerNumberSufix = " P";
    public bool crossFadingAttack = false;
    public bool wallOnLeft = false;
    public float aerialVelX = 0;
    public bool controlable = false;
    public bool countered = false;
    public GameObject miscCollidersObject;
    private Collider[] miscColliders;
    private CharacterControler otherChar;
    private GameObject throwParticles;
    private AudioSource whooshSound;

    public Rigidbody rb;


    //attack properties stucture
    
    

    void Awake()
    {
        whooshSound = GetComponents < AudioSource >()[1];
        whooshSound.time = 0.2f;

        speed = walkSpeed;
        //transform.Find("stickmanV2").Find("Cube").GetComponent<Renderer>().material.color = playerColor;
        //playerNumberSufix += playerNumber;
        currentHealth = maxHealth;
        attackProperties = AttackData.Instance.AttackProperties;
        miscColliders = new Collider[miscCollidersObject.GetComponents<Collider>().Length];
        int i = 0;
        foreach (var collider in miscCollidersObject.GetComponents<Collider>())
        {
            miscColliders[i] = collider;
            i++;
        }
    }

    public void SetupControl(int playerNumber, Color color)
    {
        this.playerNumber = playerNumber;
        this.playerColor = color;
        playerNumberSufix += playerNumber;
        playerRenderer.material.color = color;
        Debug.Log("InvertYAxisP" + playerNumber);
        invert = PlayerPrefs.GetInt("InvertYAxisP" + playerNumber);
        Debug.Log(playerNumber + " " + invert);
    }

    // Update is called once per frame
    void Update()
    {
        if (controlable)
            GatherInput();
        else
            ClearInput();
        HandleRigidBodyMass();
        if (!isKOd)
        {
            if (currentHealth <= 0)
            {
                HandleKO();
            }
            else
            {
                HandleThrowKnockout();
                GetStanceButton();
                if (throwBreakable)
                    ListenForThrowBreak();
                if (!isInThrow)
                {
                    if (otherChar != null) otherChar = null;
                    if (!isInHitStun)
                    {
                        consecutiveHits = 0;
                        comboDamage = 0;
                        GatherCombatInputs();
                        if (!isInBlockStun)
                        {
                            if (isStuckToWall)
                                HandleWallInteraction();
                            else
                                HandleMovement();
                            HandleCombat();
                        }
                        else
                            HandlePushBack();
                        HandleDoubleTapDash();
                    }
                    else
                    {
                        HandlePushBack();
                    }
                }
                HandleAnimation();
            }
        }
        else
        {
            HandleGeneralCollider();
        }
    }

    private void GatherInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal" + playerNumberSufix);
        verticalAxis = Input.GetAxisRaw("Vertical" + playerNumberSufix);
        rightPunch = Input.GetButtonDown("Right Punch" + playerNumberSufix);
        leftPunch = Input.GetButtonDown("Left Punch" + playerNumberSufix);
        rightKick = Input.GetButtonDown("Right Kick" + playerNumberSufix);
        leftKick = Input.GetButtonDown("Left Kick" + playerNumberSufix);
        stanceTrigger = Input.GetButtonDown("Stance Trigger" + playerNumberSufix);
    }

    private void ClearInput()
    {
        horizontalAxis = 0;
        verticalAxis = 0;
        rightPunch = false;
        leftPunch = false;
        rightKick = false;
        leftKick = false;
        stanceTrigger = false;
    }
    private void HandleRigidBodyMass()
    {
        if (!grounded)
            rb.mass = 100;
        else
            rb.mass = 1;
    }

    private void HandleThrowKnockout()
    {
        if (isInThrow && otherChar != null && otherChar.currentHealth <= 0)
        {
            isInThrow = false;
            otherChar.isInThrow = false;
            animator.Play(isInStance ? "StanceIdle" : "NeutralIdle");
        }
    }

    private void HandleKO()
    {
        if (!isInThrow)
        {
            rb.velocity = Vector3.zero;
            isKOd = true;
            if (hitFromFront)
                animator.CrossFade("KnockOutFront", 0.3f);
            else
                animator.CrossFade("KnockOutBack", 0.3f);
            animator.SetBool("canFloat", false);
        }
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
        if (stanceTrigger && !isInBlockStun)
        {
            if (isInStance)
            {
                isInStance = false;
            }

            else if (!isInStance)
            {
                isRunning = false;
                isInStance = true;
                if(!isAttacking)
                    FaceTheEnemy();
            }
        }
    }
    public void FaceTheEnemy()
    {
        if (otherChar == null)
            FindOtherChar();
        if ((facingLeft && otherChar.transform.position.x > transform.position.x)
                            || (!facingLeft && otherChar.transform.position.x < transform.position.x))
        {
            facingLeft = !facingLeft;
            HandleHorizontalOrientation();
        }
    }
    private void FindOtherChar()
    {
        CharacterControler[] characters = FindObjectsOfType<CharacterControler>();
        if (characters[0] != this) otherChar = characters[0];
        else otherChar = characters[1];
    }
    private void GatherCombatInputs()
    {
        if (firstInput == null)
        {
            if (leftPunch)
            {
                firstInput = "Left Punch";
            }
            else if (rightPunch)
            {
                firstInput = "Right Punch";
            }
            else if (leftKick)
            {
                firstInput = "Left Kick";
            }
            else if (rightKick)
            {
                firstInput = "Right Kick";
            }
            if (firstInput != null)
                lastCombatInputTime = Time.time;
            //if(firstInput!=null) Debug.Log("firstInput:" + firstInput);
        }
        if (firstInput != null && secondInput == null && Time.time - lastCombatInputTime <= 0.05f)
        {
            if (leftPunch)
            {
                if (firstInput != "Left Punch")
                    secondInput = "Left Punch";
            }
            if (rightPunch)
            {
                if (firstInput != "Right Punch")
                    secondInput = "Right Punch";
            }
            if (leftKick)
            {
                if (firstInput != "Left Kick")
                    secondInput = "Left Kick";
            }
            if (rightKick)
            {
                if (firstInput != "Right Kick")
                    secondInput = "Right Kick";
            }
            //Debug.Log("secondInput:" + secondInput);
        }
        if (firstInput != null && Time.time - lastCombatInputTime >= inputLeniency)
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

                    if ((firstInput == "Left Punch" && secondInput == "Left Kick") || (firstInput == "Left Kick" && secondInput == "Left Punch")
                        || (firstInput == "Right Punch" && secondInput == "Right Kick") || (firstInput == "Right Kick" && secondInput == "Right Punch"))
                    {
                        StartAttack();
                        activeLimb = "Throw";
                        isCrouching = false;
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                        animator.CrossFade("CombatThrowStart", 0.1f);
                    }
                    else
                    {
                        if (!isCrouching)
                        {
                            StartAttack();
                            if (isDashingForward)
                            {
                                crossFadingAttack = true;
                                if (firstInput == "Left Punch")
                                {
                                    animator.CrossFade("CombatDashingLeftPunch", 0.01f);
                                    SetOutputAttackProperties(attackProperties["DashingLeftPunch"]);
                                }
                                else if (firstInput == "Right Punch")
                                {
                                    animator.CrossFade("CombatDashingRightPunch", 0.01f);
                                    SetOutputAttackProperties(attackProperties["DashingRightPunch"]);
                                }
                                else if (firstInput == "Left Kick")
                                {
                                    animator.CrossFade("CombatDashingLeftKick", 0.01f);
                                    SetOutputAttackProperties(attackProperties["DashingLeftKick"]);
                                }
                                else if (firstInput == "Right Kick")
                                {
                                    animator.CrossFade("CombatDashingRightKick", 0.3f);
                                    SetOutputAttackProperties(attackProperties["DashingRightKick"]);
                                }
                            }
                            else if (isDashing && firstInput == "Left Punch")
                            {
                                crossFadingAttack = true;
                                animator.CrossFade("CombatBackdashingLeftPunch", 0.01f);
                                SetOutputAttackProperties(attackProperties["BackdashingLeftPunch"]);
                            }
                            else if (isDashing && firstInput == "Left Kick")
                            {
                                crossFadingAttack = true;
                                animator.CrossFade("CombatBackdashingLeftKick", 0.01f);
                                SetOutputAttackProperties(attackProperties["BackdashingLeftKick"]);
                            }
                            else if (isRunning && firstInput == "Right Punch")
                            {
                                crossFadingAttack = true;
                                animator.CrossFade("CombatRunningRightPunch", 0.1f);
                                SetOutputAttackProperties(attackProperties["RunningRightPunch"]);
                            }
                            else if (isRunning && firstInput == "Left Kick")
                            {
                                crossFadingAttack = true;
                                animator.CrossFade("CombatRunningLeftKick", 0.1f);
                                SetOutputAttackProperties(attackProperties["RunningLeftKick"]);
                            }
                            else if (isRunning && firstInput == "Right Kick")
                            {
                                crossFadingAttack = true;
                                animator.CrossFade("CombatRunningRightKick", 0.1f);
                                SetOutputAttackProperties(attackProperties["RunningRightKick"]);
                            }
                            else if (firstInput == "Left Punch")
                            {
                                animator.Play("CombatStandingLeftPunch");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputAttackProperties(attackProperties["StandingLeftPunch"]);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                animator.Play("CombatStandingRightPunch");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputAttackProperties(attackProperties["StandingRightPunch"]);
                            }
                            else if (firstInput == "Left Kick")
                            {
                                animator.Play("CombatStandingLeftKick");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputAttackProperties(attackProperties["StandingLeftKick"]);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                animator.Play("CombatStandingRightKick");
                                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                                SetOutputAttackProperties(attackProperties["StandingRightKick"]);
                            }
                        }
                        else
                        {
                            StartAttack();
                            if (firstInput == "Left Punch")
                            {
                                animator.Play("CombatCrouchingLeftPunch");
                                SetOutputAttackProperties(attackProperties["CrouchingLeftPunch"]);
                            }
                            else if (firstInput == "Right Punch")
                            {
                                crossFadingAttack = true;
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightPunch", 0.3f);
                                SetOutputAttackProperties(attackProperties["CrouchingRightPunch"]);
                            }
                            else if (firstInput == "Left Kick")
                            {
                                animator.Play("CombatCrouchingLeftKick");
                                SetOutputAttackProperties(attackProperties["CrouchingLeftKick"]);
                            }
                            else if (firstInput == "Right Kick")
                            {
                                crossFadingAttack = true;
                                isCrouching = false;
                                animator.CrossFade("CombatCrouchingRightKick", 0.3f);
                                SetOutputAttackProperties(attackProperties["CrouchingRightKick"]);
                            }
                        }
                    }
                }
                else if (!grounded)
                {
                    StartAttack();
                    aerialVelX = rb.velocity.x;
                    if (firstInput == "Left Punch")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingLeftPunch");
                        SetOutputAttackProperties(attackProperties["JumpingLeftPunch"]);
                    }
                    else if (firstInput == "Right Punch")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingRightPunch");
                        SetOutputAttackProperties(attackProperties["JumpingRightPunch"]);
                    }
                    else if (firstInput == "Left Kick")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingLeftKick");
                        SetOutputAttackProperties(attackProperties["JumpingLeftKick"]);
                    }
                    else if (firstInput == "Right Kick")
                    {
                        isAerialAttacking = true;
                        animator.Play("CombatJumpingRightKick");
                        SetOutputAttackProperties(attackProperties["JumpingRightKick"]);
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
            lastTime -= 1;
            isCrouching = true;
            isDashing = false;
            isAirDashing = false;
            animator.Play("CombatBadLanding");
            activeFrames = false;
        }
        if (grounded && !lastFrameGrounded && isDashingForward && isAttacking)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void StartAttack()
    {
        isCancelable = false;
        isAttacking = true;
        outgoingAttackLanded = false;
        if(isInStance)FaceTheEnemy();
    }

    private void HandleAnimation()
    {
        animator.SetFloat("vertSpeed", rb.velocity.y);
        animator.SetBool("grounded", grounded);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInStance", isInStance);
        animator.SetBool("isCrouching", isCrouching);
        if (isInHitStun || isInBlockStun || isInThrow || isAttacking || isAirDashing || isDashing)
            animator.SetBool("canFloat", false);
        else
            animator.SetBool("canFloat", true);

        if (!facingLeft)
            animator.SetFloat("horSpeed", rb.velocity.x);
        else
            animator.SetFloat("horSpeed", -rb.velocity.x);
        animator.SetFloat("speed", animationMovementSpeed);
        if (!isAttacking && !isInThrow && !isInHitStun && !isInBlockStun)
        {
            //Neutral jump/land/crouch/neutral transfer
            if (!isInStance)
            {
                if (grounded && !lastFrameGrounded && animator.GetBool("canFloat"))
                    animator.CrossFade(!isRunning ? "NeutralIdle" : "NeutralRun", 0.1f);
                if (!grounded && lastFrameGrounded && animator.GetBool("canFloat"))
                    animator.CrossFade("NeutralFloatTree", 0.1f);
                if (lastFrameStance && !isDashing)
                    if (grounded)
                        if (isCrouching && lastFrameCrouching)
                            animator.CrossFade("NeutralCrouchIdle", 0.1f);
                        else
                            animator.CrossFade("NeutralIdle", 0.1f);
                if (isCrouching && !lastFrameCrouching)
                    animator.CrossFade("NeutralCrouchIdle", 0.1f);
            }
            //Stance jump/land/crouch/stance transfer
            if (isInStance)
            {
                if (!grounded && lastFrameGrounded && animator.GetBool("canFloat"))
                    animator.CrossFade("StanceFloatTree", 0.1f);
                if (grounded && !lastFrameGrounded && animator.GetBool("canFloat"))
                    animator.CrossFade("StanceIdle", 0.1f);
                //if (!facingLeft)
                //    animator.SetFloat("horSpeed", rb.velocity.x);
                //else
                //    animator.SetFloat("horSpeed", -rb.velocity.x);
                if (!lastFrameStance && !isDashing)
                    if (grounded)
                        if (isCrouching && lastFrameCrouching)
                            animator.CrossFade("StanceCrouchIdle", 0.1f);
                        else
                            animator.CrossFade("StanceIdle", 0.1f);
                if (isCrouching && !lastFrameCrouching)
                    animator.CrossFade("StanceCrouchIdle", 0.1f);
            }
        }

        if (isDashing && isCrouching && !isAttacking)
        {
            isCancelable = true;
            isDashingForward = false;
            isDashing = false;
            animator.CrossFade("StanceCrouchIdle", 0.1f);
        }

        lastFrameGrounded = grounded;
        lastFrameCrouching = isCrouching;
        lastFrameStance = isInStance;
    }

    public bool WallInteractionCondition(Collider other)
    {
        return !grounded && !isAttacking && !isInBlockStun && !isInHitStun && !isInThrow && !isStuckToWall
            && ((transform.position.x > other.transform.position.x && horizontalAxis < 0)
            || (transform.position.x < other.transform.position.x && horizontalAxis > 0));
    }
    public void StartWallInteraction(Collider wall)
    {
        isStuckToWall = true;
        HandleGeneralCollider();
        animator.SetBool("canFloat", false);
        airDashExpanded = false;

        rb.useGravity = false;
        if (wall.transform.position.x < transform.position.x) wallOnLeft = true;
        else wallOnLeft = false;
        if ((wallOnLeft && !facingLeft) || (!wallOnLeft && facingLeft)) animator.CrossFade("WallInteractionBehind", 0.1f);
        else animator.CrossFade("WallInteractionFront", 0.1f); ;
    }
    private void StopWallInteraction()
    {
        rb.useGravity = true;
        isStuckToWall = false;
    }

    private void HandleWallInteraction()
    {
        rb.velocity = new Vector3(0, 0, 0);
        if ((wallOnLeft && horizontalAxis > 0)
            || (!wallOnLeft && horizontalAxis < 0))
        {
            rb.velocity = new Vector3(wallOnLeft ? dashHorForce : -dashHorForce, dashVertForce, 0);
            isAirDashing = true;
            if ((facingLeft && !wallOnLeft) || (!facingLeft && wallOnLeft)) animator.CrossFade("CombatStanceAirDashForward", 0.05f);
            else animator.CrossFade("CombatStanceAirDashBackward", 0.05f);
            StopWallInteraction();
        }
        if (verticalAxis * invert > 0)
        {
            rb.velocity = new Vector3(wallOnLeft ? dashVertForce : -dashVertForce, jumpForce, 0);
            isAirDashing = true;
            if ((facingLeft && !wallOnLeft) || (!facingLeft && wallOnLeft)) animator.CrossFade("CombatStanceAirDashForward", 0.05f);
            else animator.CrossFade("CombatStanceAirDashBackward", 0.05f);
            StopWallInteraction();
        }
        if (verticalAxis * invert < 0)
        {
            animator.SetBool("canFloat", true);
            animator.CrossFade("StanceFloatTree", 0.1f);
            rb.velocity = new Vector3(wallOnLeft ? dashVertForce : -dashVertForce, -5, 0);
            StopWallInteraction();
        }
        if (isAttacking)
        {
            StopWallInteraction();
        }
    }

    private void HandleMovement()
    {
        grounded = IsGrounded();
        if (!isAttacking)
        {
            if (grounded)
            {
                airDashExpanded = false;
                isAirDashing = false;
            }
            if (!isInStance)
            {

                if ((grounded || (!isInStance && !isAirDashing)) && horizontalAxis < 0)
                {
                    facingLeft = true;

                }
                else if ((grounded || (!isInStance && !isAirDashing)) && horizontalAxis > 0)
                {
                    facingLeft = false;
                }
                HandleHorizontalOrientation();

            }
            if (isInStance)
            {
                if (!facingLeft)
                    animationMovementSpeed = horizontalAxis;
                else
                    animationMovementSpeed = -horizontalAxis;
            }
            else
            {
                animationMovementSpeed = Math.Abs(horizontalAxis);
            }
            // if (grounded && !isCrouching && !isInStance && !isDashing) rb.velocity = new Vector3(Input.GetAxis("Horizontal" + playerNumberSufix) * speed, rb.velocity.y, 0);
            //else if (grounded && !isCrouching && isInStance && !isDashing) rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal" + playerNumberSufix) * speed, rb.velocity.y, 0);
            if (grounded && isCrouching)
            {
                isRunning = false;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            if (grounded && verticalAxis * invert > 0 && !isDashing)
            {
                grounded = false;
                isDashingForward = false;
                rb.velocity = (new Vector3(horizontalAxis * speed, isInStance ? stanceJumpForce : jumpForce, 0));
            }

            if (grounded && verticalAxis * invert < 0)
            {
                isCrouching = true;
            }
            else
                isCrouching = false;

        }
        //  HandleDoubleTapDash();
        LimitVelocity();
        HandleGeneralCollider();
    }

    internal void HandleHorizontalOrientation()
    {
        if (facingLeft)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
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
        if (!grounded)
        {
            miscColliders[0].enabled = false;
            miscColliders[1].enabled = false;
            miscColliders[2].enabled = false;
            miscColliders[3].enabled = false;
            miscColliders[4].enabled = false;
        }
        else
        {
            miscColliders[0].enabled = true;
            miscColliders[1].enabled = true;
            miscColliders[2].enabled = true;
        }
        if (isKOd)
        {
            standingCollider.enabled = false;
            crouchingCollider.enabled = false;
            wallInteractionCollider.enabled = false;
        }
        else if (isStuckToWall)
        {
            wallInteractionCollider.enabled = true;
            standingCollider.enabled = false;
            crouchingCollider.enabled = false;
            miscColliders[3].enabled = true;
            miscColliders[4].enabled = true;
        }
        else if (isCrouching)
        {
            crouchingCollider.enabled = true;
            standingCollider.enabled = false;
            wallInteractionCollider.enabled = false;
        }
        else
        {
            wallInteractionCollider.enabled = false;
            crouchingCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }

    private void HandleDoubleTapDash()
    {
        if (((horizontalAxis < 0 && facingLeftDash)
            ^ (horizontalAxis > 0 && !facingLeftDash))
            && Time.time - lastTime < 0.15f && !isStuckToWall && isCancelable)
        {
            if (!isInStance)
                isRunning = true;
            if (isInStance && grounded && !isDashing && !isInBlockStun && grounded)
            {
                isDashing = true;
                if ((facingLeft && horizontalAxis < 0) || (!facingLeft && horizontalAxis > 0))
                {
                    isDashingForward = true;
                    animator.CrossFade("CombatStanceDashForward", 0.1f);
                }
                else
                {
                    animator.CrossFade("CombatStanceDashBackward", 0.1f);
                }
            }
            if (!grounded && !airDashExpanded)
            {
                rb.velocity = (new Vector3(horizontalAxis * dashHorForce, dashVertForce, 0));
                airDashExpanded = true;
                isAirDashing = true;
                if ((facingLeft && rb.velocity.x < 0) || (!facingLeft && rb.velocity.x > 0))
                    animator.CrossFade("CombatStanceAirDashForward", 0.05f);
                else
                    animator.CrossFade("CombatStanceAirDashBackward", 0.05f);
            }
        }

        if (isRunning && horizontalAxis != 0)
            speed = runSpeed;
        else
        {
            speed = walkSpeed;
            isRunning = false;
        }
        if (horizontalAxis == 0 && lastFrameHorizontalAxis != 0)
        {
            lastTime = Time.time;
            if (lastFrameHorizontalAxis > 0) facingLeftDash = false;
            else if (lastFrameHorizontalAxis < 0) facingLeftDash = true;
        }

        if (horizontalAxis != 0)
            lastFrameHorizontalAxis = horizontalAxis;
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

    public void InvocationOInvulnerability()
    {
        invulnerable = true;
        Invoke("SwitchOffVulnerability", 1f);
    }
    private void SwitchOffVulnerability()
    {
        invulnerable = false;
    }
    internal void RefillHealth()
    {
        currentHealth = maxHealth;
    }
    private void SetOutputAttackProperties(AttackPropertiesStructure attackProperty)
    {
        outputDamage = attackProperty.damage;
        outputHitStun = attackProperty.hitStun;
        outputBlockStun = attackProperty.blockStun;
        outputPushBack = attackProperty.pushBack;
        outputBlockType = attackProperty.blockType;
    }

    public bool CheckBlockCondition(BlockType block)
    {
        return grounded && hitFromFront && isInStance && !isInHitStun && !isAttacking && !isDashingForward
               && ((block == BlockType.Standing && !isCrouching)
               ^ (block == BlockType.Crouching && isCrouching)
               || block == BlockType.Either)
               && ((facingLeft && horizontalAxis >= 0)
               ^ (!facingLeft && horizontalAxis <= 0));
    }

    internal void ApplyBlockStun(float inputBlockStun, string hitZone, float pushBack)
    {
        StandardIssueCombatActionConnectSwitches();
        isInBlockStun = true;
        inputPushBack = pushBack;

        animator.SetFloat("blockStun", (60 / inputBlockStun));
        //        Debug.Log("blockstun:" + ((60 / inputBlockStun) + " inframes:" + (60 / ((60 / inputBlockStun)))));
        animator.SetBool("canFloat", false);
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
        HandleHorizontalOrientation();

        StandardIssueCombatActionConnectSwitches();
        isInHitStun = true;
        inputPushBack = pushBack;

        var dmg = (int)((countered?1.2f:1)*(damage * (((1 - (consecutiveHits * 0.2f)) < 0.2) ? (0.2f) : (1 - (consecutiveHits * 0.2f)))));

        currentHealth -= dmg;
        comboDamage += dmg;

        //  Debug.Log("hit:"+consecutiveHits + " combo damage:" + comboDamage);
        animator.SetFloat("hitStun", (60 / (inputHitStun - consecutiveHits)));
        // Debug.Log("hitstun:" + ((60 / (inputHitStun - consecutiveHits))) + " inframes:" + (60/((60 /(inputHitStun - consecutiveHits)))));
        consecutiveHits += 1;
        animator.SetBool("canFloat", false);
        string animationToPlay = "";
        if (hitZone == "Head" || hitZone == "UpperSpine" || hitZone == "Arm_R" || hitZone == "Arm_L")
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
        else if (hitZone == "LowerSpine" || hitZone == "UpperLeg_L" || hitZone == "UpperLeg_R")
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
        else if (hitZone == "LowerLeg_L")
        {
            animationToPlay = "HitReactionStandingLowL";
        }
        else if (hitZone == "LowerLeg_R")
        {
            animationToPlay = "HitReactionStandingLowR";
        }
        animator.Play(animationToPlay, 0);
    }

    public void StartTheThrow(CharacterControler thrower)
    {

        otherChar = thrower;
        rb.velocity = Vector3.zero;
        otherChar.rb.velocity = Vector3.zero;

        animator.SetBool("canFloat", false);
        otherChar.animator.SetBool("canFloat", false);
        StandardIssueCombatActionConnectSwitches();
        otherChar.StandardIssueCombatActionConnectSwitches();
        isInThrow = true;
        isCrouching = false;
        HandleGeneralCollider();
        otherChar.isInThrow = true;
        throwBreakable = true;
        //float positionOffset;
        if (hitFromFront)
        {
            animator.Play("ThrowReactionForward");
            thrower.animator.Play("CombatThrowForward");
        }
        else
        {
            throwBreakable = false;
            animator.Play("ThrowReactionBack");
            thrower.animator.Play("CombatThrowBack");
        }
        throwParticles = (GameObject)Instantiate(Resources.Load("Effects/ThrowParticles/ThrowParticles"), transform);
        Destroy(throwParticles, 1.5f);
    }

    private void ListenForThrowBreak()
    {
        rb.velocity = Vector3.zero;
        if (leftPunch || rightPunch)
        {
            Destroy(throwParticles);
            throwBreakable = false;
            otherChar.animator.Play("CombatThrowBroken");
            animator.Play("ThrowReactionBreak");
        }
    }
    internal void StandardIssueCombatActionConnectSwitches()
    {
        activeFrames = false;
        crossFadingAttack = false;
        StopWallInteraction();
    }
    internal void ResetToNeutral()
    {
        currentHealth = maxHealth;
        isInThrow = false;
        isInHitStun = false;
        isInBlockStun = false;
        isInStance = false;
        isAerialAttacking = false;
        isCancelable = true;
        isAttacking = false;
        isCrouching = false;
        isDashing = false;
        isDashingForward = false;
        airDashExpanded = false;
        isRunning = false;
        crossFadingAttack = false;
        activeFrames = false;
        isKOd = false;
        outgoingAttackLanded = false;
        throwBreakable = false;
        isAirDashing = false;
        rb.velocity = Vector3.zero;
        animationMovementSpeed = 0;

        StopWallInteraction();
        HandleAnimation();
        HandleGeneralCollider();
        animator.SetBool("canFloat", true);
        animator.Play("NeutralIdle");
    }

    public void PlayWhoosh()
    {
        whooshSound.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isStuckToWall && collision.gameObject.tag == "Player" 
            && collision.gameObject != this.gameObject 
            && (collision.transform.position.y) > transform.position.y)
        {
            animator.SetBool("canFloat", true);
            animator.CrossFade("StanceFloatTree", 0.1f);
            rb.velocity = new Vector3(0, -5, 0);
            StopWallInteraction();
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    StopAerialInterference(collision);
    //}
    //private void OnCollisionStay(Collision collision)
    //{
    //    StopAerialInterference(collision);
    //}
    //private void StopAerialInterference(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Player" && collision.gameObject != this.gameObject)
    //    {
    //        CharacterControler otherChar = collision.gameObject.GetComponent<CharacterControler>();
    //        if (!grounded && !isdashing)
    //        {
    //            if (otherchar.isattacking && ((!facingleft && otherchar.facingleft && rb.velocity.x < 0) || (facingleft && !otherchar.facingleft && rb.velocity.x > 0)))
    //            {
    //                rb.velocity = new vector3(0, rb.velocity.y, rb.velocity.z);
    //            }
    //        }
    //        if (isattacking && isaerialattacking)
    //        {
    //            if (aerialvelx > 0 && otherchar.rb.velocity.x < 0 || aerialvelx < 0 && otherchar.rb.velocity.x > 0)
    //            {
    //                aerialvelx = 0;
    //            }
    //        }
    //    }
    //}
}
