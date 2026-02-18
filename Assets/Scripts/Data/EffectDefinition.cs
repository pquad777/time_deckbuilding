using System;
using UnityEngine;
public enum EffectType
{
    Damage,
    GainBlock,
    Dodge,
    ApplyFilter
}
public enum FilterType
{
    NextNAttacksBonus,
    NextEnemyHitDouble
}
[Serializable]

public class EffectDefinition
{
    public EffectType type;

    public int value;

    // 필터용
    public FilterType filterType;
    public int stacks;
    public int magnitude;
}

