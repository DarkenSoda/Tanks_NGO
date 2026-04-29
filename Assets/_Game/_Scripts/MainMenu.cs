using System;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;

    public void StartServer() => NetworkingManager.Singleton.StartServer();

    public void StartHost()
    {
        NetworkingManager.Singleton.UpdatePlayerName(playerName.text);
        NetworkingManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkingManager.Singleton.UpdatePlayerName(playerName.text);
        NetworkingManager.Singleton.StartClient();
    }
}
