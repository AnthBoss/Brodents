using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using TMPro;

//Client Side
public enum ServerToClientId : ushort
{
    sync = 1,
    playerMovement,
    acceptLogin,
    playerJoined,
    warningMessage,
    sceneChange,
    chat,
    gameJoin,
}

public enum ClientToServerId : ushort
{
    login = 1,
    input,
    chat,
    gameJoin,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
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
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }

    private ushort _serverTick;

    public ushort ServerTick
    {
        get => _serverTick;
        private set
        {
            _serverTick = value;
            InterpolationTick = (ushort)(value - TicksBetweenPositionUpdates);
        }
    }

    public ushort InterpolationTick { get; private set; }
    private ushort _ticksBetweenPositionUpdates = 2;

    [Space(10)]
    [SerializeField] private ushort tickDivergenceTolerance = 1;

    public ushort TicksBetweenPositionUpdates
    {
        get => _ticksBetweenPositionUpdates;
        private set
        {
            _ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)(_serverTick - value);
        }
    }

    private void Awake()
    {
        if(!Singleton)
        {
            Singleton = this;
        }
        DontDestroyOnLoad(this);
    }

    [SerializeField] public Dictionary<ushort, string> onlinePlayerList = new Dictionary<ushort, string>();

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;

        ServerTick = 2;
    }

    #region Prefabs

    [SerializeField] public GameObject PlayerPrefab;
    [SerializeField] public GameObject LocalPlayerPrefab;

    [SerializeField] public GameObject SkunkPrefab;

    #endregion

    private void FixedUpdate()
    {
        Client.Tick();
        ServerTick++;
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect()
    {
        Client.Connect($"{"192.168.254.45"}:{"7777"}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        if(AccountInfo.Singleton.currScreen == 0)
        {
            AccountInfo.Singleton.SendAccountInfo();
        } else
        {
            AccountInfo.Singleton.SendRegisterInfo();
        }
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        AccountInfo.Singleton.BackToMain();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //MenuController.Singleton.RemoveUser(e.Id);
        if (Singleton.onlinePlayerList.TryGetValue(e.Id, out string user))
        {
            Singleton.onlinePlayerList.Remove(e.Id);
            MenuController.Singleton.RemoveUser(user);
        }
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        AccountInfo.Singleton.BackToMain();

        foreach (Player player in Player.list.Values)
        {
            Destroy(player.gameObject);
        }
    }

    private void SetTick(ushort serverTick)
    {
        if (Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance) {
            Debug.Log($"Client tick: {ServerTick} -> {serverTick}");
            ServerTick = serverTick;
        }
    }

    [MessageHandler((ushort)ServerToClientId.sync)]
    public static void Sync(Message message)
    {
        Singleton.SetTick(message.GetUShort());
    }
}

