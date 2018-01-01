using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public EventSystem eventSystem;
    public Slider roundSlider;
    public Text rounds;
    public GameObject mainMenu;
    public GameObject matchSettings;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    public void StartMatch()
    {
        SceneManager.LoadScene("scene");
    }

    public void GoToMatchSettings()
    {
        mainMenu.SetActive(false);
        matchSettings.SetActive(true);
        eventSystem.SetSelectedGameObject(roundSlider.gameObject);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeRoundsValue()
    {
        rounds.text = "" + roundSlider.value;
        MatchSettings.Instance.MaxRounds = (int)roundSlider.value;
    }

    public void GoToMainMenu()
    {
        mainMenu.SetActive(true);
        matchSettings.SetActive(false);
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
}
