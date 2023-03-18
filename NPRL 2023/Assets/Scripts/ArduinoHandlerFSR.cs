using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoHandlerFSR : ArduinoHandler
{
    const int MAX_FSR_READING = 255;

    [SerializeField]
    float MIN_SCALE = 0;
    [SerializeField]
    float MAX_SCALE = 0;

    [SerializeField]
    float NON_CHANGING_SCALE = 0.05f;

    [SerializeField]
    Transform[] trans;

    public override void OnMessageArrived(byte[] msg, int len)
    {
        throw new NotImplementedException();
    }

    public void OnMessageArrivedStr(string str)
    {
        string[] split = str.Substring(1, str.Length - 2).Split(',');

        //Get the finger index and the FSR value.
        int idx = int.Parse(split[0]);
        int val = int.Parse(split[1]);

        //Map from val/MAX_FSR_READING to a value between MIN_ and MAX_SCALE.
        float newScale = Mathf.Lerp(MIN_SCALE, MAX_SCALE, val / MAX_FSR_READING);

        //Update the scale.
        trans[idx].localScale = new Vector3(newScale, NON_CHANGING_SCALE, NON_CHANGING_SCALE);
        trans[idx].localPosition = new Vector3(newScale / 2, 0, 0);
    }

    public override void OnConnectionEvent(bool success)
    {
        Debug.Log("Connection status: " + success);
    }
}
