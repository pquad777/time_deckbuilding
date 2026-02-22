using System;

public class DamageOverTimeFilter : IEventFilter
{
    // 이 필터는 "마커 이벤트"를 가장 먼저 잡아야 해서 Priority 낮게
    public int Priority => 0;

    private readonly int _durationTurns;
    private readonly int _damagePerTurn;

    // ScheduleCardEffects에서 마커 이벤트에 넣어줄 id
    public string ApplyEventId { get; }

    public DamageOverTimeFilter(int durationTurns, int damagePerTurn)
    {
        _durationTurns = Math.Max(1, durationTurns);
        _damagePerTurn = Math.Max(0, damagePerTurn);

        ApplyEventId = $"dot_apply_{Guid.NewGuid():N}";
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (e.id != ApplyEventId) return;
        e.cancelled = true;

        for (int t = 1; t <= _durationTurns; t++)
        {
            ctx.Schedule(t, ActionEvent.Damage(
                e.source,
                e.target,              
                _damagePerTurn,
                isAttack: false,       
                repeat: 1,
                id: $"dot_tick_{ApplyEventId}_{t}"
            ));
        }

        ctx.RemoveFilter(this);
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx) { }
}