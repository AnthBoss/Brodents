using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
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
                Debug.Log($"{nameof(GameLogic)} instance already exists, destroyed duplicate");
                Destroy(value);
            }
        }
    }

    /*

    public GameObject SkunkPrefab => skunkPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject skunkPrefab;
    [SerializeField] private GameObject porcupinePrefab;

    */

    public GameObject UserPrefab => userPrefab;
    [SerializeField] private GameObject userPrefab;

    private void Awake()
    {
        Singleton = this;
    }

    #region messages

    public void SendScene(string scene, ushort toClientId)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.sceneChange);
        message.AddString(scene);
        NetworkManager.Singleton.Server.Send((message), toClientId);
    }

    public void SendChat(string user, string chat)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.chat);
        message.AddString(user);
        Debug.Log("Sending " + chat);
        message.AddString(chat);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    [MessageHandler((ushort)ClientToServerId.chat)]
    private static void Chat(ushort fromClientId, Message message)
    {
        if (User.list.TryGetValue(fromClientId, out User user))
        {
            Singleton.SendChat(user.Username, message.GetString());
        }
    }

    [MessageHandler((ushort)ClientToServerId.gameJoin)]
    private static void GameJoin(ushort fromClientId, Message message)
    {
        if(User.list.TryGetValue(fromClientId, out User user))
        {

            Singleton.SendScene("Battle", fromClientId);

        }
    }

    #endregion
}
