using Managers;
using Unity.Netcode;
using UnityEngine;

public class DPSMine : NetworkBehaviour
{
    public int damage = 50;
    private bool firstTrigger = true;

    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip explodeSound;

    public override void OnNetworkSpawn()
    {
        if (spawnSound != null)
        {
            AudioManager.Instance.PlaySFXClip(spawnSound, transform);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (firstTrigger)
        {
            firstTrigger = false;
            return;
        }

        if (other.TryGetComponent(out NetPlayer player))
        {
            player.TakeDamageGlobal(damage, OwnerClientId);
            ExplodeRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ExplodeRpc()
    {
        if (explodeSound != null)
        {
            AudioManager.Instance.PlaySFXClip(explodeSound, transform);
        }

        if (IsServer)
        {
            NetworkObject.Despawn(true);
        }
    }
}
