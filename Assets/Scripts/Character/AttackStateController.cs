using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    private Animator animator;
    private CharacterControler controller;
    private Rigidbody parentRb;
    private float forwardMomentum;
	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        parentRb = transform.parent.GetComponent<Rigidbody>();
        controller = transform.parent.GetComponent<CharacterControler>();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}


    private void EndAttackingState()
    {
        if (!controller.crossFadingAttack)
        {
            if (controller.isInHitStun) controller.FaceTheEnemy();
            controller.countered = false;
            controller.isAttacking = false;
            controller.isCancelable = true;
            controller.isAerialAttacking = false;
            controller.isInHitStun = false;
            controller.activeFrames = false;
            controller.outgoingAttackLanded = false;
            controller.isInThrow = false;
            controller.throwBreakable = false;
            controller.isAirDashing = false;
            controller.isDashingForward = false;
            controller.isDashing = false;
            controller.animator.SetFloat("onBlockModifier", 1);
        }
        //Debug.Log("called" + Time.time);
        //forwardMomentum = 0;
    }

    private void EndBlockingState()
    {
        controller.isInBlockStun = false;
        EndAttackingState();
    }

    private void ToggleCrouch()
    {
        controller.isCrouching = !controller.isCrouching;
        controller.HandleGeneralCollider();
    }
    private void ToggleActiveFramesOn()
    {
        controller.activeFrames = true;
        controller.PlayWhoosh();
    }
    private void ToggleActiveFramesOff()
    {
        controller.activeFrames = false;
        controller.isDashing = false;
        controller.isDashingForward = false;
        controller.crossFadingAttack = false;
    }
    private void ToggleThrowUnbreakable()
    {
        controller.throwBreakable = false;
    }
    private void SetCancelability()
    {
        if(controller.outgoingAttackLanded)
            controller.isCancelable = true;
    }
    private void ApplyDamage(float damage)
    {
        controller.currentHealth -= damage;
        controller.consecutiveHits += 1;
        controller.comboDamage += damage;
    }
    private void TurnOffCancelability()
    {
        if (!controller.isCrouching)
            controller.isCancelable = false;
        else
            controller.isCancelable = true;
    }
    private void TurnAround()
    {
       controller.facingLeft = !controller.facingLeft;
    }
    private void OnAnimatorMove()
    {
        //if ((controler.isAttacking && !controler.isAerialAttacking) || controler.isKOd || controler.isInThrow || (controler.isDashing && controler.grounded))
        if (controller.grounded && !controller.isInHitStun && !controller.isInBlockStun)
        {
            parentRb.velocity = new Vector3(animator.velocity.x, parentRb.velocity.y, 0);
            if(controller.isAttacking)transform.parent.rotation *= animator.deltaRotation;
        }
        //else if((controler.isAttacking && controler.isAerialAttacking) || controler.isKOd)
          //  parentRb.velocity = new Vector3(controler.aerialVelX, parentRb.velocity.y, 0);
        //     else
        //          transform.parent.position += animator.deltaPosition;
    }
}
