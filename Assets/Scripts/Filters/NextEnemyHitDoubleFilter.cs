public class NextEnemyHitDoubleFilter : IEventFilter
{
    public int Priority => 50;
    private int _stacks;

    public NextEnemyHitDoubleFilter(int stacks = 1)
    {
        _stacks = stacks;
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (_stacks <= 0) return;

        // "적이 받는 다음 1회 데미지" = 플레이어가 적에게 주는 데미지
        if (e.type != ActionType.Damage) return;
        if (e.source != Team.Player) return;
        if (e.target != TargetPolicy.Opponent) return;

        e.value *= 2;

        _stacks--;
        if (_stacks <= 0)
            ctx.RemoveFilter(this);
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx) { }
}