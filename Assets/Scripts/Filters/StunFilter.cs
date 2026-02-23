using System;
using UnityEngine;

public class StunFilter : IEventFilter
{
    public int Priority => 1000; // 최우선으로 막기
    public int Remains { get; set; }

    public Team Owner { get; set; }

    public StunFilter(Team blockedTeam, int blocks)
    {
        Owner = blockedTeam;
        Remains = Math.Max(1, blocks);
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        // ✅ 해당 팀이 시도하는 모든 행동을 막음
        if (e.source == Owner)
        {
            e.cancelled = true;
        }
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        // ✅ 실제로 "행동 시도"가 발생하면 1회 소모 (취소된 것도 시도니까 소모)
        if (executed.source == Owner)
        {
            Remains--;
            if (Remains <= 0)
                ctx.RemoveFilter(this);
        }
    }
    public Sprite ReturnSprite()
    {
        return Resources.Load<Sprite>("Sprites/Stun");
    }
}