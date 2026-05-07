using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerClass", menuName = "ScriptableObjects/PlayerClassSO")]
public class PlayerClassSO : ScriptableObject
{
    public PlayerClass playerClass;
    public int maxHealth;
    public float moveSpeed;
    public int damage;
    public AbilitySO classAbility;
}
