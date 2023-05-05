using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArduinoHandlerHand : ArduinoHandler
{
    [Header("General Values")]

    [SerializeField]
    bool debugMode = false;

    [Header("IMU Values")]

    [SerializeField]
    Transform hand;

    private float[] euler = new float[3];

    //Debug text to display IMU values in-game.
    [SerializeField]
    Text[] displayText = new Text[6];


    [Header("FSR Values")]

    [SerializeField]
    Vector2[] fingerFSRRanges = new Vector2[5];

    string[] fingernames = new string[5] {
        "thumb_flex",
        "index_flex",
        "middle_flex",
        "ring_flex",
        "pinky_flex"
    };

    [SerializeField]
    Animator animator;

    [SerializeField]
    [Range(0f, 1f)]
    float changeSpeed = 0.1f;

    Keyboard keyboard;

    private void Start()
    {
        keyboard = Keyboard.current;
    }


    public override void OnMessageArrived(byte[] msg, int len)
    {
    }

    public void OnMessageArrivedStr(string str)
    {
        if (debugMode)
        {
            Debug.Log("MSG received: '" + str + "'");
        }

        //Split into indicator and content.
        //Indicator 0 = IMU reading
        //Indicator 1 = FSR reading
        string[] split = str.Substring(1, str.Length - 2).Split(new char[] { ',' }, 2);
        int indicator = int.Parse(split[0]);

        switch (indicator)
        {
            case 0:
                split = split[1].Split(',');

                //Get info.
                int reading_idx = int.Parse(split[0]);

                if (debugMode)
                    displayText[reading_idx].text = split[1];

                //Set the proper euler value of the hand.
                if (reading_idx >= 3)
                {
                    if (debugMode)
                        Debug.Log("IMU msg - Axis #" + (reading_idx - 3) + ", new value: " + split[1]);

                    euler[reading_idx - 3] = float.Parse(split[1]);
                    hand.localEulerAngles = new Vector3(euler[1], -euler[2], -euler[0]);
                }

                break;
            case 1:
                split = split[1].Split(',');

                //Get info.
                int finger_idx = int.Parse(split[0]);
                float val = float.Parse(split[1]);

                if (debugMode)
                    Debug.Log("FSR msg - Digit #" + finger_idx + ", value: " + val);

                //Calculate a percentage by mapping val to a range for that finger.
                float percentage = (val - fingerFSRRanges[finger_idx].x) / (fingerFSRRanges[finger_idx].y - fingerFSRRanges[finger_idx].x);

                //Apply it to the animator.
                animator.SetFloat(fingernames[finger_idx], percentage);

                break;
        }
    }

    public override void OnConnectionEvent(bool success)
    {
        Debug.Log("Connection status: " + success);
    }

    private void Update()
    {
        if (keyboard != null)
        {
            //Get input, cancel out if both are pressed
            int dir =
                (keyboard.upArrowKey.isPressed ? 1 : 0) +
                (keyboard.downArrowKey.isPressed ? -1 : 0);

            //Only move if there's a key pressed.
            if (dir != 0)
            {
                float currFlex = animator.GetFloat("index_flex");
                currFlex += dir * changeSpeed;
                animator.SetFloat("index_flex", Mathf.Clamp01(currFlex));
            }
        }
        else
        {
            keyboard = Keyboard.current;
        }
    }
}
