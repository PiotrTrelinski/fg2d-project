﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public GameObject player1;
    public GameObject player2;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3((player1.transform.position.x + player2.transform.position.x) / 2, transform.position.y, transform.position.z);
	}
}
