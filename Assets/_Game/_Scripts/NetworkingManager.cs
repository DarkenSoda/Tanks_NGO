using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingManager : NetworkManager
{
    private static NetworkingManager singleton;
    public new static NetworkingManager Singleton => singleton;

    public PlayerData PlayerData { get; private set; }
    public Dictionary<ulong, PlayerClass> ClientClasses { get; private set; } = new Dictionary<ulong, PlayerClass>();

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
        NetworkConfig.ConnectionApproval = true;
        ConnectionApprovalCallback = ApprovalCheck;
        OnServerStarted += Srvr_Started;
    }

    private void ApprovalCheck(ConnectionApprovalRequest request, ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;

        if (request.Payload != null && request.Payload.Length > 0)
        {
            int classIndex = BitConverter.ToInt32(request.Payload, 0);
            ClientClasses[request.ClientNetworkId] = (PlayerClass)classIndex;
        }
    }

    private void Srvr_Started()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void UpdatePlayerData(string name, TeamID teamID, PlayerClass playerClass)
    {
        PlayerData = new PlayerData
        {
            PlayerName = name,
            TeamID = teamID,
            PlayerClass = playerClass
        };

        NetworkConfig.ConnectionData = BitConverter.GetBytes((int)playerClass);
        ClientClasses[LocalClientId] = playerClass;
    }

    public NetPlayer GetPlayerByID(ulong id)
    {
        NetPlayer player = null;
        if (ConnectedClients.TryGetValue(id, out var client))
        {
            player = client.PlayerObject.GetComponent<NetPlayer>();
        }
        return player;
    }
}
