using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule {
    public int PlayerNumber = 1;
    protected override AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
    {
        return base.GetAxisEventData(x, y * PlayerPrefs.GetInt("InvertYAxisP"+PlayerNumber, 1), moveDeadZone);
    }
}
