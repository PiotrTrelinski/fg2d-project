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

    public int maxRounds;
    public int timeLimit;
    public Color p1Color;
    public Color p2Color;

    private MatchSettings()
    {
        maxRounds = 3;
        timeLimit = 99;
        p1Color = Color.blue;
        p2Color = Color.red;
    }

}
