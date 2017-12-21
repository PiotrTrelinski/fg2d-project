using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player1GameObject;
    private GameObject player2GameObject;
    private CharacterControler player1;
    private CharacterControler player2;
    public Text roundsWonP1;
    public Text roundsWonP2;
    private int roundsP1 = 0;
    private int roundsP2 = 0;
    public Vector3 p1StartingPosition;
    public Vector3 p2StartingPosition;
    public GameObject P1HealthBar;
    public GameObject P2HealthBar;
    public CameraScript cameraController;
    //private int winnerRounds;
    //private Text winnerRoundsText;
    private bool roundFinished = false;
	// Use this for initialization
	void Awake ()
    {
        p1StartingPosition = GameObject.Find("P1StartingPosition").transform.position;
        p2StartingPosition = GameObject.Find("P2StartingPosition").transform.position;
        player1GameObject = Instantiate(playerPrefab, p1StartingPosition, Quaternion.identity);
        player2GameObject = Instantiate(playerPrefab, p2StartingPosition, Quaternion.identity);
        player1 = player1GameObject.GetComponent<CharacterControler>(); 
        player2 = player2GameObject.GetComponent<CharacterControler>();
        P1HealthBar.GetComponentInChildren<HealthBarScript>().character = player1;
        P2HealthBar.GetComponentInChildren<HealthBarScript>().character = player2;
        player1.SetupControl(1, Color.black);
        player2.SetupControl(2, Color.red);
        player1.facingLeft = false;
        player2.facingLeft = true;
        cameraController.players = new GameObject[2];
        cameraController.players[0] = player1GameObject;
        cameraController.players[1] = player2GameObject;
        roundsWonP1.text = "Rounds won: " + roundsP1;
        roundsWonP2.text = "Rounds won: " + roundsP2; 
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (player1.currentHealth <= 0 && player2.currentHealth > 0)
        {
            if (!roundFinished)
            {
                //winnerRoundsText = roundsWonP2;
                //winnerRounds = roundsP2;
                roundsP2 += 1;
                roundsWonP2.text = "Rounds won: " + roundsP2; ;
                roundFinished = true;
                Invoke("StartNewRound", 5);
            }
        }
        if (player2.currentHealth <= 0 && player1.currentHealth > 0)
        {
            if (!roundFinished)
            {
                //winnerRoundsText = roundsWonP1;
                //winnerRounds = roundsP1;
                roundsP1 += 1;
                roundsWonP1.text = "Rounds won: " + roundsP1;
                roundFinished = true;
                Invoke("StartNewRound", 3);
            }
        }else if(player1.currentHealth <= 0 && player2.currentHealth <= 0)
        {
            if (!roundFinished)
            {
                roundFinished = true;
                Invoke("StartNewRound", 3);
            }
        }
    }

    private void StartNewRound()
    {
        //winnerRounds += 1;
        //winnerRoundsText.text = "Rounds won: " + winnerRounds;
        player1.RefillHealth();
        player2.RefillHealth();
        player1.ResetToNeutral();
        player2.ResetToNeutral();
        player1.transform.position = p1StartingPosition;
        player2.transform.position = p2StartingPosition;
        player1.facingLeft = false;
        player2.facingLeft = true;
        roundFinished = false;
    }
}
