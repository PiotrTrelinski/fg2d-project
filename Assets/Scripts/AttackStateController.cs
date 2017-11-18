using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour {

    public CharacterControler controler;
    private float forwardMomentum;
    private bool airMomentum = false;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (controler.isAttacking && !airMomentum)
            HandleMomentum();
	}

    private void HandleMomentum()
    {
        if(!controler.facingLeft)
            controler.rb.velocity = new Vector3(forwardMomentum, controler.rb.velocity.y, 0);
        else
            controler.rb.velocity = new Vector3(-forwardMomentum, controler.rb.velocity.y, 0);
    }

    private void EndAttackingState()
    {
        controler.isAttacking = false;
        controler.isCancelable = true;
        airMomentum = false;
        //Debug.Log("called" + Time.time);
        //forwardMomentum = 0;
    }

    private void SetForwardMomentum(float value)
    {
        forwardMomentum = value;
    }
    private void SetInertia()
    {
        airMomentum = true;
    }
    private void SetCrouch()
    {
        controler.isCrouching = true;
        controler.HandleGeneralCollider();
    }
    private void SetCancelability()
    {
        controler.isCancelable = true;
    }
}
