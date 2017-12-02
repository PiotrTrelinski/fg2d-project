using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    private Animator animator;
    public CharacterControler controler;
    private Rigidbody parentRb;
    private float forwardMomentum;
	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        parentRb = transform.parent.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}


    private void EndAttackingState()
    {
        controler.isAttacking = false;
        controler.isCancelable = true;
        controler.isAerialAttacking = false;
        controler.isInHitStun = false;
        //Debug.Log("called" + Time.time);
        //forwardMomentum = 0;
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
    }
    private void SetCancelability()
    {
        controler.isCancelable = true;
    }
    private void OnAnimatorMove()
    {
        if (controler.isAttacking && !controler.isAerialAttacking)
            parentRb.velocity = animator.velocity.x * Vector3.right;
   //     else
  //          transform.parent.position += animator.deltaPosition;
    }
}
