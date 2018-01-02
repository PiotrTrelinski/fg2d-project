using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public GameObject[] players;
    public float minZ = -15;
    public float maxZ = -55;
	// Use this for initialization
	void Start () {
        //PlayerPrefs.DeleteAll();
        //  QualitySettings.vSyncCount = 0;  // VSync must be disabled
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float x = (players[0].transform.position.x + players[1].transform.position.x) / 2;
        float y = (players[0].transform.position.y + players[1].transform.position.y) / 2 + 5;
        float z;
        //if ((Math.Abs(players[0].transform.position.x - players[1].transform.position.x) > Math.Abs(players[0].transform.position.y - players[1].transform.position.y)))
        //    z = -Math.Abs(players[0].transform.position.x - players[1].transform.position.x);
        //else
        //    z = -Math.Abs(players[0].transform.position.y - players[1].transform.position.y);
        z = -((players[0].transform.position - players[1].transform.position).magnitude + 8);
        if (z > minZ) z = minZ;
        if (z < maxZ) z = maxZ;
        transform.position = new Vector3(x, y, z);
	}
}
