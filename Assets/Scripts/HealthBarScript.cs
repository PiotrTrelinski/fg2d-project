using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

    public CharacterControler character;
    public RectTransform parentBar;
    private RectTransform healthBar;
    public RectTransform redHealthBar;

	// Use this for initialization
	void Start () {
        healthBar = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (redHealthBar.sizeDelta.x < healthBar.sizeDelta.x)
        {
            redHealthBar.sizeDelta = new Vector2(healthBar.sizeDelta.x, healthBar.sizeDelta.y);
        }
        healthBar.sizeDelta = new Vector2((character.currentHealth*parentBar.sizeDelta.x)/100, healthBar.sizeDelta.y);
        if(((!character.isInHitStun && !character.isInThrow) ||character.isKOd) && redHealthBar.sizeDelta.x > healthBar.sizeDelta.x)
        {
            redHealthBar.sizeDelta = new Vector2(redHealthBar.sizeDelta.x - parentBar.sizeDelta.x/100, healthBar.sizeDelta.y);
        }
	}
}
