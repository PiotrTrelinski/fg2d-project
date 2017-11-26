using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManagerScript : MonoBehaviour
{
    public CharacterControler player1;
    public CharacterControler player2;
    public Text roundsWonP1;
    public Text roundsWonP2;
    private int roundsP1 = 0;
    private int roundsP2 = 0;
    private Vector3 p1StartingPosition;
    private Vector3 p2StartingPosition;
	// Use this for initialization
	void Start ()
    {
        roundsWonP1.text = "Rounds won: " + roundsP1;
        roundsWonP2.text = "Rounds won: " + roundsP2;
        p1StartingPosition = player1.transform.position;
        p2StartingPosition = player2.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (player1.currentHealth <= 0 && player2.currentHealth > 0)
        {
            EndRound(ref roundsP2, ref roundsWonP2);
        }
        if (player2.currentHealth <= 0 && player1.currentHealth > 0)
        {
            EndRound(ref roundsP1, ref roundsWonP1);
        }
    }

    private void EndRound(ref int winnerRounds, ref Text winnerRoundsText)
    {
        winnerRounds += 1;
        winnerRoundsText.text = "Rounds won: " + winnerRounds;
        player1.RefillHealth();
        player2.RefillHealth();
        player1.transform.position = p1StartingPosition;
        player2.transform.position = p2StartingPosition;
        player1.ResetToNeutral();
        player2.ResetToNeutral();
        player1.facingLeft = false;
        player2.facingLeft = true;

    }
}
