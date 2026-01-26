using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    public List<CardDefinition> playerDeck;

    public bool isCasting;
    public int remainCastTime;
    public CardDefinition castingCard;
    public System.Action healthChange;
    public bool isDodging;

    public void ApplyDamage(int damage)
    {
        health -= damage;
        healthChange.Invoke();
    }

    public void ApplyDefense(int defense)
    {
        this.defense += defense;
        healthChange.Invoke();
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
}
