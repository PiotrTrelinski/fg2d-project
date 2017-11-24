using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingLimbScript : MonoBehaviour
{

    private CharacterControler owner;
    private CapsuleCollider col;
    public string limbLabel;

	// Use this for initialization
	void Start ()
    {
        owner = GetComponentInParent<CharacterControler>();
        col = GetComponent<CapsuleCollider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawRay(transform.position, -transform.right * 0.6f , Color.green);
	}

    private void OnTriggerEnter(Collider other)
    {
        CheckHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckHit(other);
    }
    private void CheckHit(Collider other)
    {
        if (limbLabel == owner.activeLimb)
        {
            if (owner.activeFrames)
            {
                if (other.gameObject.tag == "Damagable")
                {
                    CharacterControler otherCharacter = other.gameObject.GetComponentInParent<CharacterControler>();
                    if (otherCharacter != null && otherCharacter != owner && !otherCharacter.invulnerable)
                    {
                        Debug.Log(owner.playerNumber + " hit " + otherCharacter.playerNumber);
                        otherCharacter.invulnerable = true;
                        otherCharacter.InvocationOfInvulnerability();
                        otherCharacter.times += 1;
                        otherCharacter.currentHealth -= owner.outputDamage;
                    }
                }
            }
        }
    }
}
