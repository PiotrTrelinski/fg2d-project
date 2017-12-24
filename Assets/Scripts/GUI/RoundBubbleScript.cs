using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundBubbleScript : MonoBehaviour {

    public RawImage bubbleFill;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        bubbleFill.gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        bubbleFill.gameObject.SetActive(false);
    }
}
