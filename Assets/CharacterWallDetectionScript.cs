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
	void FixedUpdate ()
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
        if (owner.WallInteractionCondition(other) && other.gameObject.layer == 8)
        {
            owner.StartWallInteraction(other);
        }
    }
}
