using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }
    public string Name { get; private set; }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        Player player;
        if(id == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(NetworkManager.Singleton.LocalPlayerPrefab, new Vector2(0f, 0f), Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
            Debug.Log("Player spawned");
        } else
        {
            player = Instantiate(NetworkManager.Singleton.PlayerPrefab, new Vector2(0f, 0f), Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
            MenuController.Singleton.AddUser(username);
            Debug.Log("Other spawned");
        }
        player.Name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        list.Add(id, player);
    }

    #region Messages

    [MessageHandler((ushort)ServerToClientId.playerJoined)]
    public static void PlayerJoined(Message message)
    {
        ushort getshort = message.GetUShort();
        string getstring = message.GetString();
        NetworkManager.Singleton.onlinePlayerList.Add(getshort, getstring);
        MenuController.Singleton.AddUser(getstring);
        Spawn(getshort, getstring);
    }

    #endregion


}
