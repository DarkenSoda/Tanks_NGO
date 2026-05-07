using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/SpawnPrefabAbility")]
public class SpawnAbilitySO : AbilitySO
{
    public GameObject networkPrefabToSpawn;

    public override void ExecuteServer(NetPlayer player, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (networkPrefabToSpawn == null) return;

        spawnPosition.y = 0;

        GameObject instance = Instantiate(networkPrefabToSpawn, spawnPosition, spawnRotation);
        var netObj = instance.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
    }
}