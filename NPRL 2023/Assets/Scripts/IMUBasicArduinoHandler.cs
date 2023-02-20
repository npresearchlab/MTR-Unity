using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMUBasicArduinoHandler : ArduinoHandler
{
    [SerializeField]
    private Text[] texts;

    [SerializeField]
    private Transform cube;

    private float[] euler = new float[3];

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
        int idx = int.Parse(split[0]);

        texts[idx].text = split[1];

        if (idx >= 3)
        {
            euler[idx - 3] = float.Parse(split[1]);

            cube.eulerAngles = new Vector3(euler[1], -euler[2], -euler[0]);
        }
    }

    public override void OnConnectionEvent(bool success)
    {
        Debug.Log("Connection status: " + success);
    }
}
