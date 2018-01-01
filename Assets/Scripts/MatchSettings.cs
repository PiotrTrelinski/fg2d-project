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
        if (PlayerPrefs.HasKey("RoundsPerMatch"))
        {
            maxRounds = PlayerPrefs.GetInt("RoundsPerMatch");
        }
        else
        {
            PlayerPrefs.SetInt("RoundsPerMatch", 3);
        }
        if (PlayerPrefs.HasKey("TimeLimit"))
        {
            timeLimit = PlayerPrefs.GetInt("TimeLimit");
        }
        else
        {
            PlayerPrefs.SetInt("TimeLimit", 99);
        }
        p1Color = Color.blue;
        p2Color = Color.red;
    }

}
