using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player1GameObject;
    private GameObject player2GameObject;
    private CharacterControler player1;
    private CharacterControler player2;
    private int roundsP1 = 0;
    private int roundsP2 = 0;
    public Transform p1StartingPositionTransform;
    public Transform p2StartingPositionTransform;
    private Vector3 p1StartingPosition;
    private Vector3 p2StartingPosition;
    public GameObject P1HealthBar;
    public GameObject P2HealthBar;
    public CameraScript cameraController;
    public HitFeedScript hitFeedP1;
    public HitFeedScript hitFeedP2;
    public RoundCounterScript roundCounterP1;
    public RoundCounterScript roundCounterP2;
    private int maxRounds = 3;
    private int currentRound = 0;
    private float maxTime;
    private float time;
    public Text timeCounter;
    public Text roundAnnoucements;
    //private int winnerRounds;
    //private Text winnerRoundsText;
    private bool roundInProgress = false;
	// Use this for initialization
	void Awake ()
    {
        p1StartingPosition = p1StartingPositionTransform.position;
        p2StartingPosition = p2StartingPositionTransform.position;
        player1GameObject = Instantiate(playerPrefab, p1StartingPosition, Quaternion.identity);
        player2GameObject = Instantiate(playerPrefab, p2StartingPosition, Quaternion.identity);
        player1 = player1GameObject.GetComponent<CharacterControler>(); 
        player2 = player2GameObject.GetComponent<CharacterControler>();
        P1HealthBar.GetComponentInChildren<HealthBarScript>().character = player1;
        P2HealthBar.GetComponentInChildren<HealthBarScript>().character = player2;
        player1.SetupControl(1, MatchSettings.Instance.p1Color);
        player2.SetupControl(2, MatchSettings.Instance.p2Color);
        player1.facingLeft = false;
        player2.facingLeft = true;
        player1.HandleHorizontalOrientation();
        player2.HandleHorizontalOrientation();

        cameraController.players = new GameObject[2];
        cameraController.players[0] = player1GameObject;
        cameraController.players[1] = player2GameObject;

        hitFeedP1.character = player1;
        hitFeedP2.character = player2;

        maxRounds = MatchSettings.Instance.MaxRounds;
        roundCounterP1.maxRounds = maxRounds;
        roundCounterP1.Initialize();
        roundCounterP2.maxRounds = maxRounds;
        roundCounterP2.Initialize();

        maxTime = MatchSettings.Instance.TimeLimit;
        time = maxTime;

        Invoke("SetupNewRound",0.001f);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (roundInProgress)
        {
            if ((player1.currentHealth <= 0 && player2.currentHealth > 0) || (player2.currentHealth > player1.currentHealth && time <= 0))
            {
                //winnerRoundsText = roundsWonP2;
                //winnerRounds = roundsP2;
                roundsP2 += 1;
                roundInProgress = false;
                roundCounterP2.RoundWon();
                Invoke("SetupNewRound", 3);
                roundAnnoucements.text = "Player 2 Wins";
            }
            if ((player2.currentHealth <= 0 && player1.currentHealth > 0) || (player1.currentHealth > player2.currentHealth && time <= 0))
            {
                //winnerRoundsText = roundsWonP1;
                //winnerRounds = roundsP1;
                roundsP1 += 1;
                roundInProgress = false;
                roundCounterP1.RoundWon();
                Invoke("SetupNewRound", 3);
                roundAnnoucements.text = "Player 1 Wins";
            }
            else if ((player1.currentHealth <= 0 && player2.currentHealth <= 0) || (player2.currentHealth == player1.currentHealth && time <= 0))
            {
                roundInProgress = false;
                Invoke("SetupNewRound", 3);
                roundAnnoucements.text = "Draw";
            }
            if (time >= 0) time -= Time.deltaTime;
            timeCounter.text = "" + (int)time;
        }
    }

    private void SetupNewRound()
    {
        //winnerRounds += 1;
        //winnerRoundsText.text = "Rounds won: " + winnerRounds;
        if (roundsP1 >= maxRounds || roundsP2 >= maxRounds)
        {
            StartCoroutine("ReturnToMenu");
            return;
        }
        currentRound++;
        player1.controlable = false;
        player2.controlable = false;
        player1.ResetToNeutral();
        player2.ResetToNeutral();
        player1.transform.position = p1StartingPosition;
        player2.transform.position = p2StartingPosition;
        player1.facingLeft = false;
        player2.facingLeft = true;
        player1.HandleHorizontalOrientation();
        player2.HandleHorizontalOrientation();
        time = maxTime;
        timeCounter.text = "" + time;
        AnnounceNewRound();
        
    }
    private void AnnounceNewRound()
    {
        roundAnnoucements.text = "Round " + currentRound;
        StartCoroutine("StartRound");
    }
    //private IEnumerator TakePlayerControlAway()
    //{
    //    yield return new WaitForSeconds(2);
    //    player1.controlable = false;
    //    player2.controlable = false;
    //    player1.ResetToNeutral();
    //    player2.ResetToNeutral();
    //}
    private IEnumerator StartRound()
    {
        yield return new WaitForSeconds(3);
        roundInProgress = true;
        player1.controlable = true;
        player2.controlable = true;
        roundAnnoucements.text = "FIGHT!";
        StartCoroutine("ClearRoundAnnoucement");

    }
    private IEnumerator ClearRoundAnnoucement()
    {
        yield return new WaitForSeconds(0.5f);
        roundAnnoucements.text = "";  
    }
    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenu");

    }

}
