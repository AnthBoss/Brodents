using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

//Server Side
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
            } else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }

    public ushort CurrentTick { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Server = new Server();
        Server.ClientConnected += NewPlayerConnected;
        Server.ClientDisconnected += UserLeft;
        Server.Start(port, maxClientCount);
    }

    #region Prefabs

    [SerializeField] public GameObject UserPrefab;

    #endregion

    private void FixedUpdate()
    {
        Server.Tick();

        if (CurrentTick % 250 == 0)
        {
            SendSync();
        }

        CurrentTick++;
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void NewPlayerConnected(object sender, ServerClientConnectedEventArgs e)
    {
        
    }

    private void UserLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (User.list.TryGetValue(e.Id, out User user))
        {
            Destroy(User.list[e.Id].gameObject);
        }
    }

    private void SendSync()
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.sync);
        message.Add(CurrentTick);

        Server.SendToAll(message);
    }


}
