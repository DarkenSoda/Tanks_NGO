using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : NetworkBehaviour
{
    [Serializable]
    public struct ClassPrefabMapping
    {
        public PlayerClass playerClass;
        public NetworkObject prefab;
    }

    public List<ClassPrefabMapping> classPrefabs;
    private Dictionary<PlayerClass, NetworkObject> classPrefabDict = new Dictionary<PlayerClass, NetworkObject>();

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverText;

    private int redAliveCount = 0;
    private int blueAliveCount = 0;

    private void Awake()
    {
        foreach (var mapping in classPrefabs)
        {
            classPrefabDict[mapping.playerClass] = mapping.prefab;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetPlayer.OnDeathEvent += HandlePlayerDeath;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetPlayer.OnDeathEvent -= HandlePlayerDeath;
            NetworkingManager.Singleton.SceneManager.OnLoadComplete -= SpawnNextPlayer;
        }
    }

    private void Start()
    {
        if (NetworkingManager.Singleton.IsServer)
        {
            NetworkingManager.Singleton.SceneManager.OnLoadComplete += SpawnNextPlayer;

            if (NetworkingManager.Singleton.IsHost)
            {
                SpawnNextPlayer(NetworkingManager.Singleton.LocalClientId, "", LoadSceneMode.Single);
            }
        }
    }

    private void SpawnNextPlayer(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        NetworkingManager.Singleton.ClientData.TryGetValue(clientId, out PlayerData clientData);
        PlayerClass selectedClass = clientData.PlayerClass;

        if (!classPrefabDict.TryGetValue(selectedClass, out NetworkObject prefabToSpawn))
        {
            Debug.LogError($"Prefab for class {selectedClass} not found in dictionary!");
            return;
        }

        Transform spawnPoint = clientData.TeamID == TeamID.Red
            ? redSpawnPoints[UnityEngine.Random.Range(0, redSpawnPoints.Length)]
            : blueSpawnPoints[UnityEngine.Random.Range(0, blueSpawnPoints.Length)];

        NetworkObject playerInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        playerInstance.SpawnAsPlayerObject(clientId);

        if (clientData.TeamID == TeamID.Red) redAliveCount++;
        else blueAliveCount++;
    }

    private void HandlePlayerDeath(ulong deadPlayerId)
    {
        NetworkingManager.Singleton.ClientData.TryGetValue(deadPlayerId, out PlayerData deadPlayerData);
        if (deadPlayerData.TeamID == TeamID.Red) redAliveCount--;
        else blueAliveCount--;

        if (redAliveCount <= 0)
        {
            ShowGameOverRpc(TeamID.Blue);
        }
        else if (blueAliveCount <= 0)
        {
            ShowGameOverRpc(TeamID.Red);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ShowGameOverRpc(TeamID winningTeam)
    {
        string winner = winningTeam == TeamID.Red ? "Red" : "Blue";

        gameOverPanel.SetActive(true);

        string winnerColorHex = KillUIHandler.Singleton.TeamIDToHex(winningTeam);
        gameOverText.text = $"<color={winnerColorHex}>{winner}</color> Wins!";
    }

    public void MainMenu()
    {
        NetworkingManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
