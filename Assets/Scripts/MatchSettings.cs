using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings {

    private static MatchSettings instance;
    public static MatchSettings Instance
    {
        get
        {
            if (instance == null)
                instance = new MatchSettings();
            return instance;
        }
    }

    private int maxRounds;
    private int timeLimit;
    private float musicVolume;
    public int MaxRounds
    {
        get { return maxRounds; }
        set { maxRounds = value; PlayerPrefs.SetInt("RoundsPerMatch", value); }
    }
    public int TimeLimit
    {
        get { return timeLimit; }
        set { timeLimit = value; PlayerPrefs.SetInt("TimeLimit", value); }
    }
    public float MusicVolume
    {
        get { return musicVolume; }
        set { musicVolume = value; PlayerPrefs.SetFloat("MusicVolume", value); }
    }
    public Color p1Color;
    public Color p2Color;

    private MatchSettings()
    {
        Application.targetFrameRate = 60;
        maxRounds = PlayerPrefs.GetInt("RoundsPerMatch", 3);
        timeLimit = PlayerPrefs.GetInt("TimeLimit", 60);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        p1Color = Color.blue;
        p2Color = Color.red;
    }

}
