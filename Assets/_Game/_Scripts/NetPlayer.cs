using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetPlayer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>();
    private NetworkVariable<int> health = new NetworkVariable<int>();

    [SerializeField] private Transform cannonTransform;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotSpeed = 5f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Image healthBarFill;

    private Rigidbody rb;
    private bool isDead = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerData.OnValueChanged += OnPlayerDataChanged;
        health.OnValueChanged += OnHealthChanged;

        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {
            health.Value = maxHealth;
        }

        if (IsLocalPlayer)
        {
            UpdateNameRpc(NetworkingManager.Singleton.PlayerData);
        }
        else
        {
            playerNameText.text = playerData.Value.PlayerName.ToString();
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
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        healthBarFill.fillAmount = (float)newValue / (float)maxHealth;
    }

    [Rpc(SendTo.Server)]
    private void UpdateNameRpc(PlayerData newData)
    {
        playerData.Value = newData;
    }


    [Rpc(SendTo.Everyone)]
    private void ShootRpc(Vector3 position, Quaternion rotation)
    {
        var bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<Bullet>().SetOwner(OwnerClientId);
    }

    public void TakeDamage(int damage, ulong attackerID)
    {
        if (!IsServer) return;

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

        var killer = NetworkingManager.Singleton.GetPlayerByID(killerID);

        if(killer != null)
        {
            Debug.Log($"{playerData.Value.PlayerName} was killed by {killer.playerData.Value.PlayerName}");
        }
        else
        {
            Debug.Log($"{playerData.Value.PlayerName} was killed");
        }
    }
}
