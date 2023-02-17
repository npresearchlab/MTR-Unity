using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArduinoHandler : MonoBehaviour
{
    /// <summary>
    /// The singleton Arduino Handler.
    /// </summary>
    public static ArduinoHandler singleton;


    //The variables used for tracking current communications packets.
    private int nBytes;
    private byte[] pktMsg;
    private int pktIdx;


    /// <summary>
    /// Ensures only one ArduinoHandler object exists.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// The controller for Ardity.
    /// </summary>
    SerialController controller;

    public void PrehandleMessage(int msg)
    {
        //This is a new packet.
        if (nBytes == -1)
        {
            nBytes = msg;
            pktIdx = 0;

            //If the buffer isn't big enough, lengthen it.
            if (pktMsg.Length < nBytes)
            {
                pktMsg = new byte[nBytes];
            }
            //If it is, scrub the end data. Not 100% necessary in this use case, but it's good practice.
            else
            {
                for (int i = nBytes; i < pktMsg.Length; i++)
                    pktMsg[i] = 0;
            }
        }
        //Append to the existing message.
        else
        {
            pktMsg[pktIdx] = 0;
            pktIdx++;
        }

        //Handle when the entire packet has been received.
        if (pktIdx >= nBytes)
        {
            OnMessageArrived(pktMsg, nBytes);
            nBytes = -1;
        }


        //GENERAL IDEA: OLD, OUTDATED, AND NO LONGER BEING USED
        //Packets are sent in pairs from the arduino.
        //First packet is a header packet, containing information about
        //the whether it's the last packet in the sequence and
        //how many bits from the message are being used (primarily for the
        //last packet mechanism)
        //Second packet is a data packet, containing 7 bits of data.
        //The MSB in each packet is treated as a flag differentiating between
        //header and data packets.

        //When a packet is received, it's stored in a temporary variable.
        //When both temporary variables are filled, the packet pair is
        //processed and added to the sequence. When the last packet is received,
        //this sequence will be handled appropriately.
        //Additionally, the system needs to send an ACK packet to commence transmission of
        //the next packet pair.

        //TODO: figure out how packet indices will be tracked.
        //TODO: lookup Arduino serial packet protocols.
    }

    public abstract void OnMessageArrived(byte[] msg, int len);

    public abstract void OnConnectionEvent(bool success);

    /// <summary>
    /// Sets the SerialController for this handler.
    /// </summary>
    /// <param name="_controller"></param>
    public void SetController(SerialController _controller)
    {
        controller = _controller;
    }

    /// <summary>
    /// Sends a serial message on the handler's SerialController.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void SendSerialMessage(int message)
    {
        if (controller.isConnected)
            controller.SendSerialMessage(message);
        else
            Debug.LogWarning("Arduino is disconnected");
    }
}
