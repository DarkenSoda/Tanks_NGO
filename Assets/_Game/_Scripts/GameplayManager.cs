using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [Serializable]
    public struct ClassPrefabMapping
    {
        public PlayerClass playerClass;
        public NetworkObject prefab;
    }

    public List<ClassPrefabMapping> classPrefabs;
    private Dictionary<PlayerClass, NetworkObject> classPrefabDict = new Dictionary<PlayerClass, NetworkObject>();

    public Transform[] spawnPoints;
    private int playerCount = 0;

    private void Awake()
    {
        foreach (var mapping in classPrefabs)
        {
            classPrefabDict[mapping.playerClass] = mapping.prefab;
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
        if (playerCount >= spawnPoints.Length)
        {
            Debug.LogWarning("Maximum player count reached. Cannot spawn more players.");
            return;
        }

        Transform spawnPoint = spawnPoints[playerCount];

        PlayerClass selectedClass = PlayerClass.Tank;
        if (NetworkingManager.Singleton.ClientClasses.TryGetValue(clientId, out PlayerClass clientClass))
        {
            selectedClass = clientClass;
        }

        if (!classPrefabDict.TryGetValue(selectedClass, out NetworkObject prefabToSpawn))
        {
            Debug.LogError($"Prefab for class {selectedClass} not found in dictionary!");
            return;
        }

        NetworkObject playerInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        playerInstance.SpawnAsPlayerObject(clientId);
        playerCount++;
    }
}
