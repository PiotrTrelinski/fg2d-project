using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowParticleSpawner : MonoBehaviour
{

    private GameObject hitSpark;
    private Animator animator;
    private CharacterControler owner;
	// Use this for initialization
	void Awake ()
    {
        owner = GetComponentInParent<CharacterControler>();
        hitSpark = (GameObject)Resources.Load("Effects/HitSparkNormalHit");
        animator = GetComponent<Animator>();
        animator.Play(owner.hitFromFront?"ThrowFrontParticlesAnimation":"ThrowBackParticlesAnimation");
	}
	
    void SpawnSpark()
    {
        var spark = Instantiate(hitSpark, new Vector3(transform.position.x, transform.position.y, -0.8f), Quaternion.identity);
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration);
    }

}
