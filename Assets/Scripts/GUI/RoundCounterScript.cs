using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundCounterScript : MonoBehaviour {

    public int maxRounds = 3;
    private int roundsWon = 0;
    public bool leftSide = true;
    public GameObject roundBubble;
    private RoundBubbleScript[] bubbles;

    // Use this for initialization
    void Awake()
    {

    }
	// Update is called once per frame
	void Update () {
		
	}
    public void Initialize()
    {
        bubbles = new RoundBubbleScript[maxRounds];
        for (int i = 0; i < maxRounds; i++)
        {
            bubbles[i] = Instantiate(roundBubble, transform).GetComponent<RoundBubbleScript>();
            if(leftSide)
                bubbles[i].transform.Translate(30 * i, 0, 0);
            else
                bubbles[i].transform.Translate(-30 * i, 0, 0);
        }
    }
    public void RoundWon()
    {
        if (roundsWon < maxRounds)
        {
            bubbles[roundsWon].Activate();
            roundsWon++;
        }
    }
}
