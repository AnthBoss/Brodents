using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool[] inputs;
    private Vector2 clickPoint;

    void Start()
    {
        inputs = new bool[5];
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            inputs[0] = true;
        }
        if(Input.GetKey(KeyCode.D))
        {
            inputs[1] = true;
        }
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            inputs[2] = true;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            inputs[3] = true;
        }
    }

    private void FixedUpdate()
    {
        SendInput();
        for(int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }

    #region Messages

    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector2(Camera.main.ScreenToWorldPoint(new Vector2(clickPoint.x, clickPoint.y)));
        NetworkManager.Singleton.Client.Send(message);
    }

    #endregion
}
