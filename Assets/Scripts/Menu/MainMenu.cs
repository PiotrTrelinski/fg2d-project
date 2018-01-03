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
    public Slider timeLimitSlider;
    public Text timeLimit;
    public GameObject mainMenu;
    public GameObject settings;
	// Use this for initialization
	void Awake ()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        roundSlider.value = MatchSettings.Instance.MaxRounds;
        rounds.text = "" + roundSlider.value;
        timeLimitSlider.value = MatchSettings.Instance.TimeLimit/20;
        timeLimit.text = "" + timeLimitSlider.value * 20;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(eventSystem.currentSelectedGameObject == null)
        {
            if (mainMenu.activeInHierarchy)
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
            else if (settings.activeInHierarchy)
                eventSystem.SetSelectedGameObject(roundSlider.gameObject);
        }
	}

    public void StartMatch()
    {
        SceneManager.LoadScene("scene");
    }

    public void GoToSettings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
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

    public void ChangeTimeLimitValue()
    {
        timeLimit.text = "" + timeLimitSlider.value * 20;
        MatchSettings.Instance.TimeLimit = (int)timeLimitSlider.value *20;
    }

    public void GoToMainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
}
