using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingLimbScript : MonoBehaviour
{

    private CharacterControler owner;
    public string limbLabel;
    private GameObject hitSparkNormal, hitSparkMedium, hitSparkHeavy, counterSpark;
    private GameObject blockSpark;
    private float sparkZOffset = -0.8f;
    private AudioSource audioSource;
    private AudioClip[] audioClips;

	// Use this for initialization
	void Awake ()
    {
        owner = GetComponentInParent<CharacterControler>();
        hitSparkNormal = (GameObject)Resources.Load("Effects/HitSparkNormalHit");
        hitSparkMedium = (GameObject)Resources.Load("Effects/HitSparkMediumHit");
        hitSparkHeavy = (GameObject)Resources.Load("Effects/HitSparkHeavyHit");
        counterSpark = (GameObject)Resources.Load("Effects/CounterSpark");
        blockSpark = (GameObject)Resources.Load("Effects/BlockSpark");
        audioSource = transform.root.GetComponents<AudioSource>()[0];
        audioClips = new AudioClip[6];
        audioClips[0] = (AudioClip)Resources.Load("SFX/Impacts_Processed-051");
        audioClips[1] = (AudioClip)Resources.Load("SFX/Impacts_Processed-053");
        audioClips[2] = (AudioClip)Resources.Load("SFX/Impacts_Processed-054");
        audioClips[3] = (AudioClip)Resources.Load("SFX/Impacts_Processed-055");
        audioClips[4] = (AudioClip)Resources.Load("SFX/Impacts_Processed-083");
        audioClips[5] = (AudioClip)Resources.Load("SFX/Impacts_Processed-071");
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
                            audioSource.clip = audioClips[4];
                            audioSource.Play();
                            owner.animator.SetFloat("onBlockModifier", 0.5f);
                            owner.activeFrames = false;
                            var blockSparkPos = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                            GameObject ps = (GameObject)Instantiate(blockSpark, new Vector3(other.transform.root.transform.position.x + (otherCharacter.facingLeft?-1.2f:1.2f), blockSparkPos.y, sparkZOffset), Quaternion.identity);
                            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);
                            otherCharacter.ApplyBlockStun(owner.outputBlockStun, other.transform.name, owner.outputPushBack);
                        }
                        else
                        {
                            audioSource.clip = audioClips[Random.Range(0,3)];
                            audioSource.Play();
                            owner.activeFrames = false;
                            owner.outgoingAttackLanded = true;
                            var hitSparkPos = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(gameObject.GetComponent<Collider>().ClosestPointOnBounds(other.transform.position));
                            GameObject hitSpark;
                            if (otherCharacter.isAttacking)
                            {
                                audioSource.clip = audioClips[5];
                                audioSource.Play();
                                otherCharacter.countered = true;
                                var cs = Instantiate(counterSpark, new Vector3(hitSparkPos.x, hitSparkPos.y, sparkZOffset), Quaternion.identity);
                                Destroy(cs, cs.GetComponent<ParticleSystem>().main.duration);
                            }
                            else otherCharacter.countered = false;
                            var damage = (int)((owner.countered ? 1.2f : 1) * owner.outputDamage);
                            if (damage > 18) hitSpark = hitSparkHeavy; else if (damage > 14) hitSpark = hitSparkMedium; else hitSpark = hitSparkNormal;
                            GameObject ps =(GameObject) Instantiate(hitSpark, new Vector3(hitSparkPos.x, hitSparkPos.y, sparkZOffset), Quaternion.identity);
                            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);
                            otherCharacter.ApplyHitStun(owner.outputHitStun, other.transform.name, owner.outputPushBack, owner.outputDamage);
                        }
                       
                    }
                }
            }
        }
    }
}
