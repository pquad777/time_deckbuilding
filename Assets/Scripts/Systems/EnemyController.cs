using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController
{
    public string displayName;
    public Sprite sprite;
    public Animator animator;
    public int health;
    public int maxHealth;
    public int defense;
    public List<CardDefinition> enemyActionList;
    public CardDefinition castingCard;
    public int remainCastTime;
    public System.Action OnChanged;

    public void LoadInfo(EnemyDefinition enemyDefinition)
    {
        displayName = enemyDefinition.displayName;
        sprite=enemyDefinition.sprite;
        // animator=enemyDefinition.animator; 현재 animator없음
        health = enemyDefinition.health;
        maxHealth=enemyDefinition.maxHealth;
        defense=enemyDefinition.defense;
        Debug.Log(displayName);
        enemyActionList = new List<CardDefinition>(enemyDefinition.enemyActionList);
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        OnChanged?.Invoke();
    }

    public void ApplyDefense(int defense)
    {
        this.defense += defense;
        OnChanged?.Invoke();
    }
    public void StartCasting(CardDefinition def)
    {
        this.castingCard = def;
        this.remainCastTime = def.castTimeTurns;
        OnChanged?.Invoke();
    }
}
