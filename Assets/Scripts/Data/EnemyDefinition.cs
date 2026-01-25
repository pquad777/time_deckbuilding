using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Scriptable Objects/EnemyDefinition")]
public class EnemyDefinition : ScriptableObject
{
    public string displayName;
    public Sprite sprite;
    // public Animator animator;
    public int health;
    public int maxHealth;
    public int defense;
    public List<CardDefinition> enemyActionList;
}
