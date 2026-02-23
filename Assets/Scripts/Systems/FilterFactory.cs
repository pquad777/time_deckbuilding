using UnityEngine;

public static class FilterFactory
{
    public static IEventFilter Create(FilterType type, Team owner,  int stacks, int magnitude)
    {
        switch (type)
        {
            case FilterType.NextNAttacksBonus:
                return new NextNAttacksPlusDamageFilter(stacks, magnitude);

            case FilterType.NextEnemyHitDouble:
                return new NextEnemyHitDoubleFilter(stacks);
            case FilterType.Stun:
                return new StunFilter(owner, stacks);
             
            case FilterType.Weak:
            {
                // stacks = 다음 N번 공격, magnitude = 퍼센트 감소(예: 25면 25% 감소)
                float mult = Mathf.Clamp01(1f - magnitude / 100f);
                return new WeakFilter(Team.Player, stacks, mult); 
                
            }

            case FilterType.Vulnerable:
            {
                // stacks = 다음 N번 피격, magnitude = 퍼센트 증가(예: 50이면 50% 증가)
                float mult = 1f + Mathf.Max(0, magnitude) / 100f;
                return new VulnerableFilter(Team.Enemy, stacks, mult);
            }
        }

        return null;
    }
}