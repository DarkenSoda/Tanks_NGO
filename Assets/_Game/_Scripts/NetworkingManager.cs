using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingManager : NetworkManager
{
    private static NetworkingManager singleton;
    public new static NetworkingManager Singleton => singleton;

    [SerializeField] private int playerName;

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    private void Start() {
        OnServerStarted += Srvr_Started;
    }

    private void Srvr_Started()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
