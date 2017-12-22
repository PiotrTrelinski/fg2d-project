using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingLimbScript : MonoBehaviour
{

    private CharacterControler owner;
    public string limbLabel;

	// Use this for initialization
	void Awake ()
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
        
        if ((limbLabel == owner.activeLimb) 
            || ((limbLabel == "Left Punch" || limbLabel == "Right Punch")
            && owner.activeLimb == "Throw"))
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
                        if(owner.activeLimb == "Throw")
                        {
                            if(other.gameObject.name == "Head" || other.gameObject.name == "UpperSpine" || other.gameObject.name == "LowerSpine")
                                if (!otherCharacter.isCrouching && otherCharacter.grounded)
                                {
                                    owner.InvocationOInvulnerability();
                                    owner.activeFrames = false;
                                    otherCharacter.StartTheThrow(owner);
                                }
                        }
                        else if(otherCharacter.CheckBlockCondition(owner.outputBlockType))
                        {
                            owner.activeFrames = false;
                            otherCharacter.ApplyBlockStun(owner.outputBlockStun, other.transform.name, owner.outputPushBack);
                        }
                        else
                        {
                            owner.activeFrames = false;
                            owner.outgoingAttackLanded = true;
                            otherCharacter.ApplyHitStun(owner.outputHitStun, other.transform.name, owner.outputPushBack, owner.outputDamage);
                        }
                       
                    }
                }
            }
        }
    }
}
