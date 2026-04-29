using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingManager : NetworkManager
{
    private static NetworkingManager singleton;
    public new static NetworkingManager Singleton => singleton;

    public string PlayerName { get; private set; }

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    private void Start()
    {
        OnServerStarted += Srvr_Started;
    }

    private void Srvr_Started()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void UpdatePlayerName(string name)
    {
        PlayerName = name;
    }

    public NetPlayer GetPlayerByID(ulong id)
    {
        NetPlayer player = null;
        if(ConnectedClients.TryGetValue(id, out var client))
        {
            player = client.PlayerObject.GetComponent<NetPlayer>();
        }
        return player;
    }
}
