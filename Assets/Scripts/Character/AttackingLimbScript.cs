using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingLimbScript : MonoBehaviour
{

    private CharacterControler owner;
    public string limbLabel;
    private GameObject hitSpark;
    private GameObject blockSpark;

	// Use this for initialization
	void Awake ()
    {
        owner = GetComponentInParent<CharacterControler>();
        hitSpark = (GameObject)Resources.Load("Effects/HitSparkNormalHit");
        blockSpark = (GameObject)Resources.Load("Effects/BlockSpark");
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
            //Debug.DrawLine(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), Vector3.up, Color.black);
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
                            var blocksparkpos = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                            GameObject ps = (GameObject)Instantiate(blockSpark, new Vector3(other.transform.parent.position.x + (owner.facingLeft?1:-1), blocksparkpos.y, -0.8f), Quaternion.identity);
                            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);
                            otherCharacter.ApplyBlockStun(owner.outputBlockStun, other.transform.name, owner.outputPushBack);
                        }
                        else
                        {
                            owner.activeFrames = false;
                            owner.outgoingAttackLanded = true;
                            var hitsparkpos = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(gameObject.GetComponent<Collider>().ClosestPointOnBounds(other.transform.position));
                            GameObject ps =(GameObject) Instantiate(hitSpark, hitsparkpos, Quaternion.identity);
                            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);
                            otherCharacter.ApplyHitStun(owner.outputHitStun, other.transform.name, owner.outputPushBack, owner.outputDamage);
                        }
                       
                    }
                }
            }
        }
    }
}
