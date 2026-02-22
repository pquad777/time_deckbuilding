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
            
        }

        return null;
    }
}