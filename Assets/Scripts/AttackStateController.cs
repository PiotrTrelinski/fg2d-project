using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    private Animator animator;
    public CharacterControler controler;
    private float forwardMomentum;
	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
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
        
        transform.parent.position += animator.deltaPosition.x * Vector3.right;
    }
}
