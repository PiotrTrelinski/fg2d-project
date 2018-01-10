using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowParticleSpawner : MonoBehaviour
{
    private int i = 0;
    private GameObject[] hitSpark;
    private Animator animator;
    private CharacterControler owner;
	// Use this for initialization
	void Awake ()
    {
        owner = GetComponentInParent<CharacterControler>();
        hitSpark = new GameObject[3];
        hitSpark[0] = (GameObject)Resources.Load("Effects/HitSparkNormalHit");
        hitSpark[1] = (GameObject)Resources.Load("Effects/HitSparkMediumHit");
        hitSpark[2] = (GameObject)Resources.Load("Effects/HitSparkHeavyHit");
        animator = GetComponent<Animator>();
        animator.Play(owner.hitFromFront?"ThrowFrontParticlesAnimation":"ThrowBackParticlesAnimation");
	}
	
    void SpawnSpark()
    {
        var spark = Instantiate(hitSpark[i++], new Vector3(transform.position.x, transform.position.y, -0.8f), Quaternion.identity);
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration);
    }

}
