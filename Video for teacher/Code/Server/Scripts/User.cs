using RiptideNetworking;
using RiptideNetworking.Utils;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ServerSide
public class User : MonoBehaviour
{
    public static Dictionary<ushort, User> list = new Dictionary<ushort, User>();
    public static Dictionary<ushort, User> inGameList = new Dictionary<ushort, User>();
    public ushort Id { get; private set; }
    public string Username { get; private set; }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string name)
    {
        foreach(User otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }

        User user = Instantiate(GameLogic.Singleton.UserPrefab, new Vector2(0f, 0f), Quaternion.identity).GetComponent<User>();
        user.name = $"Player {id} ({(string.IsNullOrEmpty(name) ? "Guest" : name)}";
        user.Id = id;
        user.Username = string.IsNullOrEmpty(name) ? $"Guest {id}" : name;

        user.SendSpawned();
        list.Add(id, user);
    }

    public static void SpawnInGame(ushort id, string name)
    {
        foreach(User otherPlayer in inGameList.Values)
        {
            //otherPlayer.SendSpawnedInGame(id);
        }

        
        
    }

    #region Messages

    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerJoined)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerJoined)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        return message;
    }

    [MessageHandler((ushort)ClientToServerId.login)]
    private static void Login(ushort fromClientId, Message message)
    {
        int num = message.GetInt();
        if(num == 0)
        {
            AuthManager.Singleton.LoginAttempt(message.GetString(), message.GetString(), fromClientId);
        } else if(num == 1)
        {
            AuthManager.Singleton.RegisterAttempt(message.GetString(), message.GetString(), message.GetString(), message.GetString(), fromClientId);
        } else
        {
            Spawn(fromClientId, message.GetString());
        }
    }

    #endregion
}
