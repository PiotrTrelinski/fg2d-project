using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour {

    public AudioSource audioSource;
    private bool grounded = true;
    private bool lastGrounded = true;
    private CharacterControler character;

    // Use this for initialization
    private void Awake()
    {
        character = transform.root.GetComponent<CharacterControler>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8 && !character.isInHitStun && character.controlable)
            audioSource.Play();
    }
}
