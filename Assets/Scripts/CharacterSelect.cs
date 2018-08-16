using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public CanvasScaler scaler;

    public GameObject[] colors;
    public GameObject cursorP1;
    public GameObject cursorP2;
    public Renderer p1Renderer;
    public Renderer p2Renderer;
    public AudioSource p1AudioSource;
    public AudioSource p2AudioSource;
    public AudioClip moveClip;
    public AudioClip selectClip;
    public Text annoucementText;
    public GameObject stageSelectPanel;
    public GameObject characterSelectPanel;
    public EventSystem eventSystem;
    public GameObject firstStage;

    public RawImage stagePicture;
    public Texture[] stageImages;
    

    private int p1CursorPosition = 0;
    private int p2CursorPosition = 7;
    private bool p1Selected = false;
    private bool p2Selected = false;
    private bool p1Switchable = true;
    private bool p2Switchable = true;
    private string selectedStage = "";

    private enum CharacterSelectStage
    {
        SideChosing,
        CharacterPicking,
        StagePicking
    }

    private CharacterSelectStage stage;

	// Use this for initialization
	void Awake ()
    {
        //cursorP1.transform.position = new Vector3(colors[p1CursorPosition].transform.position.x - 15 / (scaler.referenceResolution.x / Screen.width), cursorP1.transform.position.y, cursorP1.transform.position.z);
        //cursorP2.transform.position = new Vector3(colors[p2CursorPosition].transform.position.x + 15 / (scaler.referenceResolution.x / Screen.width), cursorP2.transform.position.y, cursorP2.transform.position.z);
        p1Renderer.material.color = colors[p1CursorPosition].GetComponent<Image>().color;
        p2Renderer.material.color = colors[p2CursorPosition].GetComponent<Image>().color;
        stage = CharacterSelectStage.CharacterPicking;
        stageSelectPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (stage == CharacterSelectStage.CharacterPicking)
        { 
            CharacterPicking();
        }
        if(stage == CharacterSelectStage.StagePicking)
        {
            if (eventSystem.enabled && eventSystem.currentSelectedGameObject == null)
            {
                if (stageSelectPanel.activeInHierarchy)
                    eventSystem.SetSelectedGameObject(firstStage);
            }
        }
    }

    private void CharacterPicking()
    {
        if (Input.GetButtonDown("Left Kick P1") && !p1Selected)
        {
            P1SoundSelect();   
            p1Selected = true;
            var color = colors[p1CursorPosition].GetComponent<Image>().color;
            MatchSettings.Instance.p1Color = color;
            var colorOffset = (p1CursorPosition == 0) ? (-100 / 255f) : (100 / 255f);
            color.r = color.r - colorOffset;
            color.g = color.g - colorOffset;
            color.b = color.b - colorOffset;
            if (!p2Selected)
                colors[p1CursorPosition].GetComponent<Image>().color = color;
            else
            {
                SwitchToStageSelect();
            }
            IEnumerator coroutine = SelectionEffect(cursorP1.GetComponentInChildren<Text>());
            StartCoroutine(coroutine);
        }
        if (Input.GetButtonDown("Left Kick P2") && !p2Selected)
        {
            P2SoundSelect();
            p2Selected = true;
            var color = colors[p2CursorPosition].GetComponent<Image>().color;
            MatchSettings.Instance.p2Color = color;
            var colorOffset = (p2CursorPosition == 0) ? (-100 / 255f) : (100 / 255f);
            color.r = color.r - colorOffset;
            color.g = color.g - colorOffset;
            color.b = color.b - colorOffset;
            if (!p1Selected)
                colors[p2CursorPosition].GetComponent<Image>().color = color;
            else
            {
                SwitchToStageSelect();
            }
            IEnumerator coroutine = SelectionEffect(cursorP2.GetComponentInChildren<Text>());
            StartCoroutine(coroutine);
        }
        if (Input.GetAxisRaw("Horizontal P1") != 0 && p1Switchable && !p1Selected)
        {
            P1SoundCursor();
            p1CursorPosition += Input.GetAxisRaw("Horizontal P1") > 0 ? 1 : -1;
            p1CursorPosition = p1CursorPosition > 7 ? 0 : (p1CursorPosition < 0 ? 7 : p1CursorPosition);
            cursorP1.transform.position = new Vector3(colors[p1CursorPosition].transform.position.x - 15 / (scaler.referenceResolution.x / Screen.width), cursorP1.transform.position.y, cursorP1.transform.position.z);
            p1Renderer.material.color = colors[p1CursorPosition].GetComponent<Image>().color;
            StartCoroutine("SwitchCooldownP1");
        }
        if (Input.GetAxisRaw("Horizontal P2") != 0 && p2Switchable && !p2Selected)
        {
            P2SoundCursor();
            p2CursorPosition += Input.GetAxisRaw("Horizontal P2") > 0 ? 1 : -1;
            p2CursorPosition = p2CursorPosition > 7 ? 0 : (p2CursorPosition < 0 ? 7 : p2CursorPosition);
            cursorP2.transform.position = new Vector3(colors[p2CursorPosition].transform.position.x + 15 / (scaler.referenceResolution.x / Screen.width), cursorP2.transform.position.y, cursorP2.transform.position.z);
            p2Renderer.material.color = colors[p2CursorPosition].GetComponent<Image>().color;
            StartCoroutine("SwitchCooldownP2");
        }
    }

    private void SwitchToStageSelect()
    {
        stage = CharacterSelectStage.StagePicking;
        StartCoroutine("SwitchToStageSelectCoroutine");   
    }
    private IEnumerator SwitchToStageSelectCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        annoucementText.text = "CHOSE THE STAGE";
        characterSelectPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(firstStage);
    }


    private void P1SoundCursor()
    {
        p1AudioSource.clip = moveClip;
        p1AudioSource.Play();
    }
    private void P1SoundSelect()
    {
        p1AudioSource.clip = selectClip;
        p1AudioSource.Play();
    }
    private void P2SoundCursor()
    {
        p2AudioSource.clip = moveClip;
        p2AudioSource.Play();
    }
    private void P2SoundSelect()
    {
        p2AudioSource.clip = selectClip;
        p2AudioSource.Play();
    }

    private void StartMatch()
    {
        StartCoroutine("StartGame");
        StartCoroutine("GameStartCountdown");
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(selectedStage);
    }
    private IEnumerator GameStartCountdown()
    {
        annoucementText.text = "GAME STARTING IN \n3";
        yield return new WaitForSeconds(1);
        annoucementText.text = "GAME STARTING IN \n2";
        yield return new WaitForSeconds(1);
        annoucementText.text = "GAME STARTING IN \n1";
        yield return new WaitForSeconds(1);
        annoucementText.text = "GAME STARTING IN \n0";
    }

    private IEnumerator SwitchCooldownP1()
    {
        p1Switchable = false;
        yield return new WaitForSeconds(0.2f);
        p1Switchable = true;
    }
    private IEnumerator SwitchCooldownP2()
    {
        p2Switchable = false;
        yield return new WaitForSeconds(0.2f);
        p2Switchable = true;
    }
    private IEnumerator SelectionEffect(Text text)
    {
        text.fontSize = 40;
        while (text.fontSize > 30)
        {
            var f = text.fontSize;
            text.fontSize = f - 1;
            yield return new WaitForFixedUpdate();
        }
        
    }

    public void OnStageSelect(int stageNumber)
    {
        P1SoundSelect();
        selectedStage = "Stage" + stageNumber;
        eventSystem.enabled = false;
        StartMatch();
    }
    public void OnStageHighlight(int stageNumber)
    {
        P1SoundCursor();
        stagePicture.texture = stageImages[stageNumber - 1];
    }
}
