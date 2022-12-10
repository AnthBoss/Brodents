using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatScrollingText : MonoBehaviour
{
    private static ChatScrollingText _singleton;
    public static ChatScrollingText Singleton
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
                Debug.Log($"{nameof(ChatScrollingText)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    [Header("Text Settings")]
    [SerializeField][TextArea] public List<string> itemInfo = new List<string>();

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI itemInfoText;
    [SerializeField] public GameObject chatPrefab;

    /*[MessageHandler((ushort)ServerToClientId.chat)]
    private static void Chat(Message message)
    {
        //Debug.Log(message.GetString() + " " + message.GetString());
        //Singleton.itemInfo.Add(message.GetString() + " " + message.GetString());
        GameObject text;
        text = Instantiate(Singleton.chatPrefab, new Vector2(0f, 20f), Quaternion.identity);
        text.GetComponent<TMP_Text>().text = (message.GetString() + ": " + message.GetString());
        text.transform.SetParent(Singleton.gameObject.transform);
    }*/
}
