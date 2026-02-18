using System;
using System.Collections.Generic;

public class ResolveContext
{
    public readonly CombatController Combat;

    public ResolveContext(CombatController combat)
    {
        Combat = combat;
    }

    // 필터 제거/추가(필요해질 때 사용)
    public void RemoveFilter(IEventFilter filter) => Combat.RemoveFilter(filter);
    public void AddFilter(IEventFilter filter) => Combat.AddFilter(filter);

    // 예약 이벤트 취소(나중에 “피격 시 시전 취소” 같은 것에서 사용)
    public void CancelScheduledById(string id) => Combat.CancelScheduledById(id);

    // 미래 턴에 추가 예약(채널링/지속효과에 유용)
    public void Schedule(int delay, ActionEvent e) => Combat.Schedule(delay, e);
}