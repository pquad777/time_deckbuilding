using System;
using UnityEngine;
public enum EffectType
{
    Damage,
    GainBlock,
    Dodge,
    ApplyFilter,
    DotDamage,
    AllCostDamage,
}
public enum FilterType
{
    NextNAttacksBonus,
    NextEnemyHitDouble,
    Stun,
    Weak,
    Vulnerable
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
    public TargetPolicy filterTarget;
}

