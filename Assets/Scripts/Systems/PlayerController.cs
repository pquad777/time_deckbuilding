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
    public bool isDodging;
    public int castLockUntilTurn = -1;
    public event Action OnPlayerDataChanged;
    public int actionBlockedTurns; // 0이면 정상, 1 이상이면 행동 불가

    public bool IsActionBlocked => actionBlockedTurns > 0;
    
    public void ApplyDamage(int damage, bool isPlaySound = true)
    {
        if (damage <= 0) return;
        if (isDodging) return;

        defense -= damage;
        if (defense < 0)
        {
            health += defense; // defense 음수만큼 체력 감소
            defense = 0;
        }
        health = Mathf.Max(0, health);
        if (isPlaySound)
        {
            GameManager.instance.AudioManager.PlaySfx(AudioType.TakeDamage);
        }

        OnPlayerDataChanged?.Invoke();
    }
    public void ApplyDefense(int amount)
    {
        if (amount <= 0) return;
        defense += amount;
        GameManager.instance.AudioManager.PlaySfx(AudioType.Defence);
        OnPlayerDataChanged?.Invoke();
    }
    public void GainCost(int amount)
    {
        cost = Mathf.Min(maxCost, cost + amount);
        OnPlayerDataChanged?.Invoke();
    }

    public void SetDefense(int value)
    {
        defense = Mathf.Max(0, value);
        GameManager.instance.AudioManager.PlaySfx(AudioType.Defence);
        OnPlayerDataChanged?.Invoke();
    }
    
    
    public void SpendCost(int _cost)
    {
        this.cost -= _cost;
        OnPlayerDataChanged?.Invoke();
    }
    public bool TrySpendCost(int amount)
    {
        if (amount < 0) return false;
        if (cost < amount) return false;
        cost -= amount;
        OnPlayerDataChanged?.Invoke();
        return true;
    }
    

    public void ApplyDodge()
    {
        isDodging = true;
        GameManager.instance.AudioManager.PlaySfx(AudioType.Evade);
        OnPlayerDataChanged?.Invoke();
    }

    public void EndDodge()
    {
        isDodging = false;
        OnPlayerDataChanged?.Invoke();
    }
    
    public bool CanAfford(int cost) => gold >= cost;

    public bool TrySpend(int cost)
    {
        if (gold < cost) return false;
        gold -= cost;
        GameManager.instance.AudioManager.PlaySfx(AudioType.UseGold);
        OnPlayerDataChanged?.Invoke();
        return true;
    }

    public void AddCard(CardDefinition card)
    {
        playerDeck.Add(card);
        GameManager.instance.AudioManager.PlaySfx(AudioType.GetCard);
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
    public bool IsLocked(int currentTurn) => isCasting && currentTurn < castLockUntilTurn;

    public void StartCastLock(int currentTurn, int castTimeTurns)
    {
        if (castTimeTurns <= 0) return;
        isCasting = true;
        castLockUntilTurn = currentTurn + castTimeTurns;
        OnPlayerDataChanged?.Invoke();
    }

    public void EndCastLock()
    {
        isCasting = false;
        castLockUntilTurn = -1;
        OnPlayerDataChanged?.Invoke();
    }
    
    public void AddActionBlock(int turns)
    {
        if (turns <= 0) return;
        actionBlockedTurns += turns; // 중첩 허용 (원하면 Max로 변경)
        OnPlayerDataChanged?.Invoke();
    }

    public void TickActionBlock()
    {
        if (actionBlockedTurns <= 0) return;
        actionBlockedTurns--;
        OnPlayerDataChanged?.Invoke();
    }
}
