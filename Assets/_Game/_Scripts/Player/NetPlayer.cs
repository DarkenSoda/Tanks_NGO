using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetPlayer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>();
    private NetworkVariable<int> health = new NetworkVariable<int>();

    [SerializeField] private PlayerClassSO playerClassData;

    [SerializeField] private Transform cannonTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotSpeed = 120f;
    [SerializeField] private float bodyRotSpeed = 360f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Image healthBarFill;

    [Header("Team Visuals")]
    [SerializeField] private Material redTeamMaterial;
    [SerializeField] private Material blueTeamMaterial;

    private Rigidbody rb;
    private bool isDead = false;

    public static event System.Action<ulong> OnDeathEvent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerData.OnValueChanged += OnPlayerDataChanged;
        health.OnValueChanged += OnHealthChanged;

        rb = GetComponent<Rigidbody>();

        if (IsLocalPlayer)
        {
            UpdatePlayerDataRpc(NetworkingManager.Singleton.PlayerData);
            Camera.main.GetComponent<CameraFollow>().target = bodyTransform;
        }
        else
        {
            playerNameText.text = playerData.Value.PlayerName.ToString();
            UpdateTeamColor(playerData.Value.TeamID);
        }

        if (playerClassData != null)
        {
            maxHealth = playerClassData.maxHealth;
            moveSpeed = playerClassData.moveSpeed;
        }

        if (IsServer)
        {
            health.Value = maxHealth;
        }

        OnHealthChanged(0, health.Value);
    }

    private void Update()
    {
        if (!IsLocalPlayer || isDead) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);

        // Movement
        Vector3 movement = new Vector3(x, 0, z) * (moveSpeed * Time.deltaTime);
        rb.Move(rb.position + movement, rb.rotation);

        // Body Rotation
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetRotation, bodyRotSpeed * Time.deltaTime);
        }

        // Cannon Rotation
        if (right || left)
        {
            float rotationAngle = 0;
            if (right) rotationAngle += rotSpeed * Time.deltaTime;
            if (left) rotationAngle -= rotSpeed * Time.deltaTime;
            cannonTransform.Rotate(0, rotationAngle, 0);
        }

        // Shooting
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootRpc(bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    private void OnPlayerDataChanged(PlayerData previousValue, PlayerData newValue)
    {
        playerNameText.text = newValue.PlayerName.ToString();
        UpdateTeamColor(newValue.TeamID);
    }

    private void UpdateTeamColor(TeamID teamID)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        Material teamMaterial = teamID == TeamID.Red ? redTeamMaterial : blueTeamMaterial;
        foreach (var renderer in renderers)
        {
            renderer.material = teamMaterial;
        }
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        healthBarFill.fillAmount = (float)newValue / (float)maxHealth;
    }

    [Rpc(SendTo.Server)]
    private void UpdatePlayerDataRpc(PlayerData newData)
    {
        playerData.Value = newData;
    }


    [Rpc(SendTo.Everyone)]
    private void ShootRpc(Vector3 position, Quaternion rotation)
    {
        var bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<Bullet>().SetOwner(OwnerClientId, playerClassData.damage);
    }

    public void TakeDamage(int damage, ulong attackerID)
    {
        if (!IsServer) return;

        var attacker = NetworkingManager.Singleton.GetPlayerByID(attackerID);
        if (attacker != null && attacker.playerData.Value.TeamID == playerData.Value.TeamID)
        {
            return;
        }

        health.Value -= damage;
        if (health.Value <= 0)
        {
            health.Value = 0;
            KillPlayerRPC(attackerID);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void KillPlayerRPC(ulong killerID)
    {
        isDead = true;

        OnDeathEvent?.Invoke(OwnerClientId);

        var killer = NetworkingManager.Singleton.GetPlayerByID(killerID);

        if (killer != null)
        {
            KillUIHandler.Singleton.DisplayKill(
                killer.playerData.Value.PlayerName.ToString(),
                killer.playerData.Value.TeamID,
                playerData.Value.PlayerName.ToString(),
                playerData.Value.TeamID
            );
        }
        else
        {
            KillUIHandler.Singleton.DisplayKillUnknown(
                playerData.Value.PlayerName.ToString(),
                playerData.Value.TeamID
            );
        }

        gameObject.SetActive(false);
    }
}
