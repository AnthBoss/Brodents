using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    private static MenuController _singleton;
    public static MenuController Singleton
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
                Debug.Log($"{nameof(MenuController)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private GameObject onlineTextPrefab;

    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject lobby;

    [SerializeField] public GameObject playerList;

    [SerializeField] private Dictionary<string, TMP_Text> dictItem = new Dictionary<string, TMP_Text>();

    public void Start()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Screen.SetResolution(1920, 1080, true);
        usernameText.text = "Welcome, " + AccountInfo.Singleton.usernameTag + "!";
        StartCoroutine(LateStart());
    }

    public void Awake()
    {
        Singleton = this;
    }

    public IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (string otherPlayers in NetworkManager.Singleton.onlinePlayerList.Values)
        {
            TMP_Text text;
            text = Instantiate(onlineTextPrefab, new Vector2(0f, 0f), Quaternion.identity).GetComponent<TMP_Text>();
            text.text = otherPlayers;
            text.transform.SetParent(playerList.transform, false);
            dictItem.Add(otherPlayers, text);
        }
    }

    public void JoinButtonPressed()
    {
        mainMenu.gameObject.SetActive(false);
        lobby.gameObject.SetActive(true);
    }

    public void AddUser(string name)
    {
        TMP_Text text;
        text = Instantiate(onlineTextPrefab, new Vector2(0f, 0f), Quaternion.identity).GetComponent<TMP_Text>();
        text.text = name;
        text.transform.SetParent(playerList.transform, false);
    }

    public void RemoveUser(string name)
    {
        Debug.Log("trying");
        if(dictItem.TryGetValue(name, out TMP_Text text))
        {
            Debug.Log("success");
            Destroy(text.gameObject);
        }
    }

    public void BackToMenuPressed()
    {
        mainMenu.gameObject.SetActive(true);
        lobby.gameObject.SetActive(false);
    }

    public void LogoutPressed()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Screen.SetResolution(960, 540, true);
        NetworkManager.Singleton.Client.Disconnect();
        Destroy(AccountInfo.Singleton.gameObject);
        SceneManager.LoadScene("Authenticate");
    }

    public void JoinGamePressed()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.gameJoin);
        NetworkManager.Singleton.Client.Send(message);
    }


}
