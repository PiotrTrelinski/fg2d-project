using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowParticleSpawner : MonoBehaviour
{
    private int i = 0;
    private GameObject[] hitSpark;
    private GameObject counterSpark;
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
        counterSpark = (GameObject)Resources.Load("Effects/CounterSpark");
        animator = GetComponent<Animator>();
        animator.Play(owner.hitFromFront?"ThrowFrontParticlesAnimation":"ThrowBackParticlesAnimation");
	}
	
    void SpawnSpark()
    {
        var spark = Instantiate(hitSpark[i++], new Vector3(transform.position.x, transform.position.y, -0.8f), Quaternion.identity);
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration);
        var cs = Instantiate(counterSpark, new Vector3(transform.position.x, transform.position.y, -0.8f), Quaternion.identity);
        Destroy(cs, cs.GetComponent<ParticleSystem>().main.duration);
        GetComponent<AudioSource>().Play();
        owner.PlayWhoosh();
    }

}
