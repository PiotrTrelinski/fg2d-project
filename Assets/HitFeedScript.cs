using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitFeedScript : MonoBehaviour {

    public CharacterControler character;
    private Text text;

	// Use this for initialization
	void Awake ()
    {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (character.isInHitStun || (character.isInThrow && character.consecutiveHits >= 1))
        {
            text.text = character.consecutiveHits + " HIT!\n" + character.comboDamage;
        }
        else
        {
            text.text = "";
        }
	}
}
