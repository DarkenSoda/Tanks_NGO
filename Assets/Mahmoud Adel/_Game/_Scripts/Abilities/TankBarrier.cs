using Managers;
using Unity.Netcode;
using UnityEngine;

public class TankBarrier : NetworkBehaviour
{
    public float lifetime = 5f;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip destroySound;

    public override void OnNetworkSpawn()
    {
        if (spawnSound != null)
        {
            AudioManager.Instance.PlaySFXClip(spawnSound, transform);
        }

        if (IsServer)
        {
            Invoke(nameof(DestroyWall), lifetime);
        }
    }

    private void DestroyWall()
    {
        DestroyWallRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void DestroyWallRpc()
    {
        if (destroySound != null)
        {
            AudioManager.Instance.PlaySFXClip(destroySound, transform);
        }
        
        if (IsServer)
        {
            NetworkObject.Despawn(true);
        }
    }
}