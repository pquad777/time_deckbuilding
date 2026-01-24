using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public string name;
    public Sprite sprite;
    public Animator animator;
    public int health;
    public int maxHealth;
    public int defense;
    public int cost;
    public int maxCost;
    public List<CardDefinition> playerDeckRaw;

    public bool isCasting;
    public int remainCastTime;
    public CardDefinition castingCard;

    public bool isDodging;

    public void ApplyDamage(int damage)
    {
        health -= damage;
    }

    public void ApplyDefense(int defense)
    {
        this.defense += defense;
    }

    public void ApplyDodge()
    {
        isDodging = true;
    }

    public void EndDodge()
    {
        isDodging = false;
    }

    public void StartCasting(CardDefinition def)
    {
        isCasting=true;
        castingCard = def;
    }
}
