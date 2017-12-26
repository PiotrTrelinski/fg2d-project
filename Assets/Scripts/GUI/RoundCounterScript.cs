using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        var scaler = gameObject.GetComponentInParent<CanvasScaler>();
        for (int i = 0; i < maxRounds; i++)
        {
            bubbles[i] = Instantiate(roundBubble, transform).GetComponent<RoundBubbleScript>();
            bubbles[i].transform.Translate(((leftSide ? bubbles[i].GetComponent<RectTransform>().rect.width:-bubbles[i].GetComponent<RectTransform>().rect.width) 
                * i * 1.5f)/ (scaler.referenceResolution.x / Screen.width), 0, 0);
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
    public void ResetRounds()
    {
        foreach (var bubble in bubbles)
            bubble.Deactivate();
    }
}
