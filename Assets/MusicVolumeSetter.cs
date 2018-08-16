using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeSetter : MonoBehaviour
{
    private AudioSource musicPlayer;
	void Awake()
    {
        musicPlayer = GetComponent<AudioSource>();
        musicPlayer.volume = MatchSettings.Instance.MusicVolume;
	}
}
