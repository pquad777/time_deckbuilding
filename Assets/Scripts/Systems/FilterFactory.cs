public static class FilterFactory
{
    public static IEventFilter Create(FilterType type, int stacks, int magnitude)
    {
        switch (type)
        {
            case FilterType.NextNAttacksBonus:
                return new NextNAttacksPlusDamageFilter(stacks, magnitude);

            case FilterType.NextEnemyHitDouble:
                return new NextEnemyHitDoubleFilter(stacks);
            case FilterType.DamageOverTime:
                return new DamageOverTimeFilter(stacks, magnitude);
        }

        return null;
    }
}