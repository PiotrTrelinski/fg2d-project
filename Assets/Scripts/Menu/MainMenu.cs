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
    public Toggle invertYAxisP1;
    public Toggle invertYAxisP2;
    public GameObject mainMenu;
    public GameObject settings;

    public AudioSource menuSound;
    public AudioClip cursorClip;
    public AudioClip buttonClip;

	// Use this for initialization
	void Awake ()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        roundSlider.value = MatchSettings.Instance.MaxRounds;
        rounds.text = "" + roundSlider.value;
        timeLimitSlider.value = MatchSettings.Instance.TimeLimit/20;
        timeLimit.text = "" + timeLimitSlider.value * 20;
        invertYAxisP1.isOn = (PlayerPrefs.GetInt("InvertYAxisP1", 1) == -1);
        invertYAxisP2.isOn = (PlayerPrefs.GetInt("InvertYAxisP2", 1) == -1);
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
        eventSystem.transform.GetComponent<CustomStandaloneInputModule>().enabled = false;
        StartCoroutine("StartMatchCoroutine");
    }

    private IEnumerator StartMatchCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("CharacterSelect");
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

    public void SwitchYAxisInversionP1()
    {
        int inversion = invertYAxisP1.isOn ? -1 : 1;
        PlayerPrefs.SetInt("InvertYAxisP1", inversion);
    }

    public void SwitchYAxisInversionP2()
    {
        int inversion = invertYAxisP1.isOn ? -1 : 1;
        PlayerPrefs.SetInt("InvertYAxisP2", inversion);
    }
    public void GoToMainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
    public void CursorEnter()
    {
        menuSound.clip = cursorClip;
        menuSound.Play();
    }
    public void CursorClick()
    {
        menuSound.clip = buttonClip;
        menuSound.Play();
    }
}
