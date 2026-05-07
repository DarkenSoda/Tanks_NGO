using UnityEngine;
using Unity.Netcode;

public abstract class AbilitySO : ScriptableObject
{
    public string abilityName;
    public float cooldownTime;
    public Sprite abilityIcon;

    public abstract void ExecuteServer(NetPlayer player, Vector3 spawnPosition, Quaternion spawnRotation);
}