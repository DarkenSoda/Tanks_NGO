using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public NetworkObject playerPrefab;
    public Transform[] spawnPoints;
    private int playerCount = 0;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnNextPlayer;

            if (NetworkManager.Singleton.IsHost)
            {
                SpawnNextPlayer(NetworkManager.Singleton.LocalClientId, "", LoadSceneMode.Single);
            }
        }
    }

    private void SpawnNextPlayer(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (playerCount >= spawnPoints.Length)
        {
            Debug.LogWarning("Maximum player count reached. Cannot spawn more players.");
            return;
        }

        Transform spawnPoint = spawnPoints[playerCount];
        NetworkObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        playerInstance.SpawnAsPlayerObject(clientId);
        playerCount++;
    }
}
