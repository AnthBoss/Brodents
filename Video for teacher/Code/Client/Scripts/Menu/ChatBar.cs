using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBar : MonoBehaviour
{
    private static ChatBar _singleton;
    public static ChatBar Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ChatBar)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    [SerializeField] private TMP_InputField chat;
    [SerializeField] private GameObject chatBox;
    [SerializeField] private GameObject textPrefab;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    #region messages

    public void SendMessage()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.chat);
        message.AddString(chat.GetComponent<TMP_InputField>().text);
        NetworkManager.Singleton.Client.Send(message);
    }

    [MessageHandler((ushort)ServerToClientId.chat)]
    private static void InstantiateChat(Message message)
    {
        //Debug.Log(message.GetString() + " " + message.GetString());
        //Singleton.itemInfo.Add(message.GetString() + " " + message.GetString());
        TMP_Text text;
        text = Instantiate(Singleton.textPrefab, new Vector2(0f, 0f), Quaternion.identity).GetComponent<TMP_Text>();
        text.text = (message.GetString() + ": " + message.GetString());
        text.transform.SetParent(Singleton.chatBox.transform, false);
    }

    #endregion

}
