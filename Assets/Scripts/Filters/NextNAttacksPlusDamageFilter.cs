using UnityEngine;

public class NextNAttacksPlusDamageFilter : IEventFilter
{
    public int Priority => 40;
    public int Remains { get; set; }

    private readonly int _bonus;
    public Team Owner { get; set; }
    public NextNAttacksPlusDamageFilter(int count, int bonus,Team owner)
    {
        Owner = owner;
        Remains = count;
        _bonus = bonus;
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        // 플레이어가 적에게 주는 "공격" 데미지 이벤트만 강화
        if (e.type != ActionType.Damage) return;
        if (e.source != Owner) return;
        if (e.target != TargetPolicy.Opponent) return;
        if (!e.isAttack) return;

        e.value += _bonus;

        Remains--;
        if (Remains <= 0)
            ctx.RemoveFilter(this);
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx) { }
    public Sprite ReturnSprite()
    {
        return Resources.Load<Sprite>("Sprites/Bullseye");
    }
}