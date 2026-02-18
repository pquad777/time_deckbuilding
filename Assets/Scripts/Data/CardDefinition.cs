using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "Card")]
public class CardDefinition : ScriptableObject
{
    public string displayName;
    public int cost;
    public int castTimeTurns;
    public List<EffectDefinition> effects;
    
    public Sprite artwork;
    public Sprite frame;
    public string cardText;
}

public enum CardType
{
    Attack,
    Defense,
    Dodge
}
