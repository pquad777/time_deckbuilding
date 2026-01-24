using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "Scriptable Objects/CardDefinition")]
public class CardDefinition : ScriptableObject
{
    public string displayName;
    public CardType Type;
    public int cost;
    public int castTimeTurns;
    public int power;
    public int buffDurationTurns;
    public Sprite cardSprite;
    public string cardText;
}

public enum CardType
{
    Attack,
    Defense,
    Dodge
}
