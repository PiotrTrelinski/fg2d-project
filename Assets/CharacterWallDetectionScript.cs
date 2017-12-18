using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWallDetectionScript : MonoBehaviour {

    private CharacterControler owner;
    private Collider[] colliders;
	// Use this for initialization
	void Start ()
    {
        owner = GetComponentInParent<CharacterControler>();
        colliders = GetComponents<Collider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (owner.isStuckToWall)
        {
            foreach(var collider in colliders)
            {
                collider.isTrigger = false;
            }
        }
        else
        {
            foreach (var collider in colliders)
            {
                collider.isTrigger = true;
            }
        }
	}
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("collider:" + colliders[1].bounds.center.x + " other:" + other.bounds.max.x + " condition:" + (colliders[1].bounds.center.x + 0.01f > other.bounds.max.x));
        if (owner.WallInteractionCondition(other, colliders) && other.gameObject.layer == 8)
        {
            owner.StartWallInteraction(other);
        }
    }
}
