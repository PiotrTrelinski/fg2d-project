using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

    public CharacterControler character;
    public RectTransform parentBar;
    private RectTransform healthBar;

	// Use this for initialization
	void Start () {
        healthBar = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        healthBar.sizeDelta = new Vector2((character.currentHealth*parentBar.sizeDelta.x)/100, healthBar.sizeDelta.y);
	}
}
