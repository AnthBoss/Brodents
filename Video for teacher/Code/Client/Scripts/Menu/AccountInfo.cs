using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AccountInfo : MonoBehaviour
{
    private static AccountInfo _singleton;
    public static AccountInfo Singleton
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
                Debug.Log($"{nameof(AccountInfo)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this);
    }

    public int currScreen;

    //objects
    public GameObject loginUI;
    public GameObject registerUI;
    public TMP_Text warning;

    //login
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    //register
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;

    [SerializeField] public string usernameTag;

    private void ConnectButton()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        NetworkManager.Singleton.Connect();
    }

    //Called from NetworkManager

    public void SendAccountInfo()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.login);
        message.AddInt(currScreen);
        message.AddString(emailLoginField.GetComponent<TMP_InputField>().text);
        message.AddString(passwordLoginField.GetComponent<TMP_InputField>().text);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SendRegisterInfo()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.login);
        message.AddInt(currScreen);
        message.AddString(emailRegisterField.GetComponent<TMP_InputField>().text);
        message.AddString(passwordRegisterField.GetComponent<TMP_InputField>().text);
        message.AddString(passwordRegisterVerifyField.GetComponent<TMP_InputField>().text);
        message.AddString(usernameRegisterField.GetComponent<TMP_InputField>().text);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void BackToMain()
    {

    }

    #region messages

    [MessageHandler((ushort)ServerToClientId.acceptLogin)]
    private static void LoginAccepted(Message message)
    {
        if(message.GetBool())
        {
            Singleton.ResendInfo(message.GetString());
        }
    }

    public void ResendInfo(string username)
    {
        usernameTag = username;

        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.login);
        message.AddInt(2);
        message.AddString(username);
        NetworkManager.Singleton.Client.Send(message);
    }

    [MessageHandler((ushort)ServerToClientId.warningMessage)]
    private static void Warning(Message message)
    {
        //warningLoginText.GetComponent<TextMeshProUGUI>().text = message.GetString();
        if(AccountInfo.Singleton.currScreen == 0)
        {
            AccountInfo.Singleton.loginUI.SetActive(true);
        } else
        {
            AccountInfo.Singleton.registerUI.SetActive(true);
        }
        Singleton.warning.GetComponent<TMP_Text>().text = message.GetString();
        //Debug.Log(message.GetString());
    }

    [MessageHandler((ushort)ServerToClientId.sceneChange)]
    private static void SceneChange(Message message)
    {
        SceneManager.LoadScene(message.GetString());
    }

    #endregion

    public void RegisterForumPressed()
    {
        warning.GetComponent<TMP_Text>().text = "";
        currScreen = 1;
        StartCoroutine(LoginLerpAway());
        StartCoroutine(RegisterLerp());
    }

    public void LoginForumPressed()
    {
        warning.GetComponent<TMP_Text>().text = "";
        currScreen = 0;
        StartCoroutine(RegisterLerpAway());
        StartCoroutine(LoginLerp());
    }

    public Vector2 loginUIInitialPosition;
    public Vector2 registerUIInitialPosition;

    public void Start()
    {
        loginUIInitialPosition = loginUI.gameObject.transform.localPosition;
        registerUIInitialPosition = registerUI.gameObject.transform.localPosition;
    }

    private IEnumerator LoginLerpAway()
    {
        float startTime = 0f;
        float duration = 0.05f;
        Vector2 startPosition = loginUI.gameObject.transform.localPosition;
        while (startTime < duration)
        {
            float t = startTime / duration;
            t = t * t * (3f - 2f * t);
            loginUI.gameObject.transform.localPosition = Vector2.Lerp(startPosition, registerUIInitialPosition, t);
            startTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RegisterLerp()
    {
        yield return new WaitForSeconds(0.02f);
        float startTime = 0f;
        float duration = 0.05f;
        Vector2 startPosition = registerUI.gameObject.transform.localPosition;
        while (startTime < duration)
        {
            float t = startTime / duration;
            t = t * t * (3f - 2f * t);
            registerUI.gameObject.transform.localPosition = Vector2.Lerp(startPosition, loginUIInitialPosition, t);
            startTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LoginLerp()
    {
        yield return new WaitForSeconds(0.02f);
        float startTime = 0f;
        float duration = 0.05f;
        Vector2 startPosition = loginUI.gameObject.transform.localPosition;
        while (startTime < duration)
        {
            float t = startTime / duration;
            t = t * t * (3f - 2f * t);
            loginUI.gameObject.transform.localPosition = Vector2.Lerp(startPosition, loginUIInitialPosition, t);
            startTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RegisterLerpAway()
    {
        float startTime = 0f;
        float duration = 0.05f;
        Vector2 startPosition = registerUI.gameObject.transform.localPosition;
        while (startTime < duration)
        {
            float t = startTime / duration;
            t = t * t * (3f - 2f * t);
            registerUI.gameObject.transform.localPosition = Vector2.Lerp(startPosition, registerUIInitialPosition, t);
            startTime += Time.deltaTime;
            yield return null;
        }
    }
}
