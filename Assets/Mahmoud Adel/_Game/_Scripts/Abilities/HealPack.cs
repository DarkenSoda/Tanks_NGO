using Managers;
using Unity.Netcode;
using UnityEngine;

public class HealPack : NetworkBehaviour
{
    public int healAmount = 25;

    private bool firstTrigger = true;

    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private GameObject healVFX;

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

        if(firstTrigger)
        {
            firstTrigger = false;
            return;
        }

        if (other.TryGetComponent(out NetPlayer player))
        {
            player.Heal(healAmount);
            HealRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void HealRpc()
    {
        if (healSound != null)
        {
            AudioManager.Instance.PlaySFXClip(healSound, transform);
        }
        if (healVFX != null)
        {
            VFXManager.Instance.PlayVFX(healVFX, transform.position);
        }

        if (IsServer)
        {
            NetworkObject.Despawn(true);
        }
    }
}