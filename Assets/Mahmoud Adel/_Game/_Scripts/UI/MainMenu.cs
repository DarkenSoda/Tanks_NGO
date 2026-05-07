using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_Dropdown teamID;
    [SerializeField] private TMP_Dropdown playerClass;

    private void Start()
    {
        teamID.options.Clear();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (TeamID id in Enum.GetValues(typeof(TeamID)))
        {
            options.Add(new TMP_Dropdown.OptionData(id.ToString()));
        }
        teamID.AddOptions(options);

        playerClass.options.Clear();
        List<TMP_Dropdown.OptionData> classOptions = new List<TMP_Dropdown.OptionData>();
        foreach (PlayerClass c in Enum.GetValues(typeof(PlayerClass)))
        {
            classOptions.Add(new TMP_Dropdown.OptionData(c.ToString()));
        }
        playerClass.AddOptions(classOptions);
    }

    public void StartServer() => NetworkingManager.Singleton.StartServer();

    public void StartHost()
    {
        NetworkingManager.Singleton.UpdatePlayerData(playerName.text, (TeamID)teamID.value, (PlayerClass)playerClass.value);
        NetworkingManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkingManager.Singleton.UpdatePlayerData(playerName.text, (TeamID)teamID.value, (PlayerClass)playerClass.value);
        NetworkingManager.Singleton.StartClient();
    }
}
