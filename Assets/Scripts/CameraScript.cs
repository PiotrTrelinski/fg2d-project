using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    private GameObject[] players;
	// Use this for initialization
	void Start () {
        //PlayerPrefs.DeleteAll();
        //  QualitySettings.vSyncCount = 0;  // VSync must be disabled
        players = GameObject.FindGameObjectsWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3((players[0].transform.position.x + players[1].transform.position.x) / 2, (players[0].transform.position.y + players[1].transform.position.y) /2 + 5, transform.position.z);
	}
}
