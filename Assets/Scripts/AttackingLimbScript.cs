using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingLimbScript : MonoBehaviour
{

    private CharacterControler owner;
    public string limbLabel;

	// Use this for initialization
	void Start ()
    {
        owner = GetComponentInParent<CharacterControler>();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void OnTriggerEnter(Collider other)
    {
        CheckHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckHit(other);
    }
    private void OnTriggerExit(Collider other)
    {
        CheckHit(other);
    }
    private void CheckHit(Collider other)
    {
        
        if (limbLabel == owner.activeLimb)
        {
            //Debug.Log(owner.name);
            if (owner.activeFrames)
            {
                if (other.gameObject.tag == "Damagable")
                {
                    CharacterControler otherCharacter = other.gameObject.GetComponentInParent<CharacterControler>();
                    if (otherCharacter != null && otherCharacter != owner && !otherCharacter.invulnerable && !otherCharacter.isKOd)
                    {
                       // Debug.Log(owner.playerNumber + " hit " + otherCharacter.playerNumber);
                        if((otherCharacter.facingLeft && otherCharacter.transform.position.x > owner.transform.position.x) 
                            || (!otherCharacter.facingLeft && otherCharacter.transform.position.x < owner.transform.position.x))
                        {
                            otherCharacter.hitFromFront = true;
                        }
                        else
                        {
                            otherCharacter.hitFromFront = false;
                        }
                        if (otherCharacter.grounded && otherCharacter.hitFromFront && otherCharacter.isInStance && !otherCharacter.isInHitStun && !otherCharacter.isAttacking
                            && ((owner.outputBlockType == BlockType.Standing && !otherCharacter.isCrouching) 
                            ^ (owner.outputBlockType == BlockType.Crouching && otherCharacter.isCrouching)
                            || owner.outputBlockType == BlockType.Either)
                            && ((otherCharacter.facingLeft && otherCharacter.rb.velocity.x >= 0) ^ (!otherCharacter.facingLeft && otherCharacter.rb.velocity.x <= 0)))
                        {
                            otherCharacter.ApplyBlockStun(owner.outputBlockStun, other.transform.name, owner.outputPushBack);
                        }
                        else
                        {
                            owner.outgoingAttackLanded = true;
                            otherCharacter.ApplyHitStun(owner.outputHitStun, other.transform.name, owner.outputPushBack, owner.outputDamage);
                        }
                       
                    }
                }
            }
        }
    }
}
