﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private int p1CursorPosition = 0;
    private int p2CursorPosition = 7;
    private bool p1Selected = false;
    private bool p2Selected = false;

	// Use this for initialization
	void Awake ()
    {
        //cursorP1.transform.position = new Vector3(colors[p1CursorPosition].transform.position.x - 15 / (scaler.referenceResolution.x / Screen.width), cursorP1.transform.position.y, cursorP1.transform.position.z);
        //cursorP2.transform.position = new Vector3(colors[p2CursorPosition].transform.position.x + 15 / (scaler.referenceResolution.x / Screen.width), cursorP2.transform.position.y, cursorP2.transform.position.z);
        p1Renderer.material.color = colors[p1CursorPosition].GetComponent<Image>().color;
        p2Renderer.material.color = colors[p2CursorPosition].GetComponent<Image>().color;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Left Kick P1") && !p1Selected)
        {
            p1Selected = true;
            var color = colors[p1CursorPosition].GetComponent<Image>().color;
            MatchSettings.Instance.p1Color = color;
            var colorOffset = (p1CursorPosition == 0) ? (-50 / 255f) : (50 / 255f);
            if (!p2Selected)
                colors[p1CursorPosition].GetComponent<Image>().color = new Color(color.r - colorOffset, color.g - colorOffset, color.b - colorOffset);
            else
                StartCoroutine("StartGame");
        }
        if (Input.GetButtonDown("Left Kick P2") && !p2Selected)
        {
            p2Selected = true;
            var color = colors[p2CursorPosition].GetComponent<Image>().color;
            MatchSettings.Instance.p2Color = color;
            var colorOffset = (p2CursorPosition == 0) ? (-50 / 255f) : (50 / 255f);
            if (!p1Selected)
                colors[p2CursorPosition].GetComponent<Image>().color = new Color(color.r - colorOffset, color.g - colorOffset, color.b - colorOffset);
            else
                StartCoroutine("StartGame");
        }
        if (Input.GetButtonDown("Horizontal P1") && !p1Selected)
        {
            p1CursorPosition += Input.GetAxisRaw("Horizontal P1") > 0 ? 1 : -1;
            p1CursorPosition = p1CursorPosition > 7 ? 0 : (p1CursorPosition < 0 ? 7 : p1CursorPosition);
            cursorP1.transform.position = new Vector3(colors[p1CursorPosition].transform.position.x - 15 / (scaler.referenceResolution.x / Screen.width), cursorP1.transform.position.y, cursorP1.transform.position.z);
            p1Renderer.material.color = colors[p1CursorPosition].GetComponent<Image>().color;
        }
        if (Input.GetButtonDown("Horizontal P2") && !p2Selected)
        {
            p2CursorPosition += Input.GetAxisRaw("Horizontal P2") > 0 ? 1 : -1;
            p2CursorPosition = p2CursorPosition > 7 ? 0 : (p2CursorPosition < 0 ? 7 : p2CursorPosition);
            cursorP2.transform.position = new Vector3(colors[p2CursorPosition].transform.position.x + 15 / (scaler.referenceResolution.x / Screen.width), cursorP2.transform.position.y, cursorP2.transform.position.z);
            p2Renderer.material.color = colors[p2CursorPosition].GetComponent<Image>().color;
        }            
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("scene");
    }
}
