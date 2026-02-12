using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController: MonoBehaviour
{
    public string name;
    [SerializeField]public Sprite sprite;

    public Sprite Sprite => sprite;
    // public Animator animator;
    public int health;
    public int maxHealth;
    public int defense;
    public int cost;
    public int maxCost;
    public int gold = 10000;
    public List<CardDefinition> playerDeck;

    public bool isCasting;
    public int remainCastTime;
    public CardDefinition castingCard;
    public System.Action healthChange;
    public System.Action CostChange;
    public bool isDodging;
    public event Action OnPlayerDataChanged;

    public void ApplyDamage(int damage)
    {
        if (isDodging)
            return;
        this.defense -= damage;
        if(this.defense<0)
        health +=this.defense;
        this.defense = 0;
        healthChange.Invoke();
        OnPlayerDataChanged?.Invoke();
    }

    public void ApplyDefense(int defense)
    {
        this.defense += defense;
        healthChange.Invoke();
        OnPlayerDataChanged?.Invoke();
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
        remainCastTime = def.castTimeTurns;
    }
    public bool CanAfford(int cost) => gold >= cost;

    public bool TrySpend(int cost)
    {
        if (gold < cost) return false;
        gold -= cost;
        OnPlayerDataChanged?.Invoke();
        return true;
    }

    public void AddCard(CardDefinition card)
    {
        playerDeck.Add(card);
        OnPlayerDataChanged?.Invoke();
    }

    public bool TryHeal(int healAmount, int cost)
    {
        if (health >= maxHealth) return false;
        if (!TrySpend(cost)) return false;

        health = Mathf.Min(maxHealth, health + healAmount);
        OnPlayerDataChanged?.Invoke();
        return true;
    }
}
