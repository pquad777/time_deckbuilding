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
    
    public void LoadInfo(EnemyDefinition enemyDefinition)
    {
        displayName = enemyDefinition.displayName;
        sprite=enemyDefinition.sprite;
        animator=enemyDefinition.animator;
        health = enemyDefinition.health;
        maxHealth=enemyDefinition.maxHealth;
        defense=enemyDefinition.defense;
        Debug.Log(displayName);
        enemyActionList = new List<CardDefinition>(enemyDefinition.enemyActionList);
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
    }

    public void ApplyDefense(int defense)
    {
        this.defense += defense;
    }
    public void StartCasting(CardDefinition def)
    {
        this.castingCard = def;
        this.remainCastTime = def.castTimeTurns;
    }
}
