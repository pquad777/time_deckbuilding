public class NextNAttacksPlusDamageFilter : IEventFilter
{
    public int Priority => 40;

    private int _remain;
    private readonly int _bonus;

    public NextNAttacksPlusDamageFilter(int count, int bonus)
    {
        _remain = count;
        _bonus = bonus;
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (_remain <= 0) return;

        // 플레이어가 적에게 주는 "공격" 데미지 이벤트만 강화
        if (e.type != ActionType.Damage) return;
        if (e.source != Team.Player) return;
        if (e.target != TargetPolicy.Opponent) return;
        if (!e.isAttack) return;

        e.value += _bonus;

        _remain--;
        if (_remain <= 0)
            ctx.RemoveFilter(this);
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx) { }
}