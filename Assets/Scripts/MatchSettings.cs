using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings {

    private static MatchSettings instance;
    public static MatchSettings Instance
    {
        get
        {
            if(instance == null)
                instance = new MatchSettings();
            return instance;
        }
    }

    private int maxRounds;
    private int timeLimit;
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
    public Color p1Color;
    public Color p2Color;

    private MatchSettings()
    {
        maxRounds = PlayerPrefs.GetInt("RoundsPerMatch", 3);
        timeLimit = PlayerPrefs.GetInt("TimeLimit", 60);
        p1Color = Color.blue;
        p2Color = Color.red;
    }

}
