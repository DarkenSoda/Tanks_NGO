using TMPro;
using UnityEngine;

public class KillUIHandler : MonoBehaviour
{
    private static KillUIHandler singleton;
    public static KillUIHandler Singleton => singleton;

    [SerializeField] private GameObject killMessagePrefab;
    [SerializeField] private Transform killMessageParent;
    [SerializeField] private Color redTeamColor = Color.red;
    [SerializeField] private Color blueTeamColor = Color.blue;
    [SerializeField] private float messageDuration = 3f;

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    public void DisplayKill(string killerName, TeamID killerTeam, string killedName, TeamID killedTeam)
    {
        string killerColorHex = TeamIDToHex(killerTeam);
        string killedColorHex = TeamIDToHex(killedTeam);

        GameObject killMessageInstance = Instantiate(killMessagePrefab, killMessageParent);
        killMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"<color={killerColorHex}>{killerName}</color> killed <color={killedColorHex}>{killedName}</color>";
        Destroy(killMessageInstance, messageDuration);
    }

    public void DisplayKillUnknown(string killedName, TeamID killedTeam)
    {
        string killedColorHex = TeamIDToHex(killedTeam);

        GameObject killMessageInstance = Instantiate(killMessagePrefab, killMessageParent);
        killMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"<color={killedColorHex}>{killedName}</color> was killed";
        Destroy(killMessageInstance, messageDuration);
    }

    private string TeamIDToHex(TeamID teamID)
    {
        Color color = teamID == TeamID.Red ? redTeamColor : blueTeamColor;
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }
}
