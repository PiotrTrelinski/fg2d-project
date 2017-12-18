using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    private Animator animator;
    private CharacterControler controler;
    private Rigidbody parentRb;
    private float forwardMomentum;
	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        parentRb = transform.parent.GetComponent<Rigidbody>();
        controler = transform.parent.GetComponent<CharacterControler>();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}


    private void EndAttackingState()
    {
        if (!controler.crossFadingAttack)
        {
            controler.isAttacking = false;
            controler.isCancelable = true;
            controler.isAerialAttacking = false;
            controler.isInHitStun = false;
            controler.activeFrames = false;
            controler.outgoingAttackLanded = false;
            controler.isInThrow = false;
            controler.throwBreakable = false;
            controler.isAirDashing = false;
            controler.isDashingForward = false;
            controler.isDashing = false;
        }
        //Debug.Log("called" + Time.time);
        //forwardMomentum = 0;
    }

    private void EndBlockingState()
    {
        controler.isInBlockStun = false;
        EndAttackingState();
    }

    private void ToggleCrouch()
    {
        controler.isCrouching = ! controler.isCrouching;
        controler.HandleGeneralCollider();
    }
    private void ToggleActiveFramesOn()
    {
        controler.activeFrames = true;
    }
    private void ToggleActiveFramesOff()
    {
        controler.activeFrames = false;
        controler.isDashing = false;
        controler.isDashingForward = false;
        controler.crossFadingAttack = false;
    }
    private void ToggleThrowUnbreakable()
    {
        controler.throwBreakable = false;
    }
    private void SetCancelability()
    {
        if(controler.outgoingAttackLanded)
            controler.isCancelable = true;
    }
    private void ApplyDamage(float damage)
    {
        controler.currentHealth -= damage;
    }
    private void TurnOffCancelability()
    {
        if(!controler.isCrouching)
            controler.isCancelable = false;
    }
    private void OnAnimatorMove()
    {
        if ((controler.isAttacking && !controler.isAerialAttacking) || controler.isKOd || controler.isInThrow || controler.isDashing)
            parentRb.velocity = new Vector3(animator.velocity.x, parentRb.velocity.y, 0);
        //else if((controler.isAttacking && controler.isAerialAttacking) || controler.isKOd)
          //  parentRb.velocity = new Vector3(controler.aerialVelX, parentRb.velocity.y, 0);
        //     else
        //          transform.parent.position += animator.deltaPosition;
    }
}
