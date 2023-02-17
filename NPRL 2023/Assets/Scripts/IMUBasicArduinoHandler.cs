using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMUBasicArduinoHandler : ArduinoHandler
{
    [SerializeField]
    private Text[] texts;

    public override void OnMessageArrived(byte[] msg, int len)
    {
        //Convert each floating point value into a string and display it.
        for (int i = 0; i < len && i/sizeof(float) < texts.Length; i += sizeof(float))
        {
            texts[i / sizeof(float)].text = BitConverter.ToSingle(msg, i).ToString();
        }
    }

    public void OnMessageArrivedStr(string str)
    {
        string[] split = str.Substring(1, str.Length - 2).Split(',');
        texts[int.Parse(split[0])].text = split[1];
    }

    public override void OnConnectionEvent(bool success)
    {
        Debug.Log("Connection status: " + success);
    }
}
