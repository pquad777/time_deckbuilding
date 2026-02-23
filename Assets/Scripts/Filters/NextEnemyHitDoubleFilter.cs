using UnityEngine;

public class NextEnemyHitDoubleFilter : IEventFilter
{
    public int Priority => 50;
    public int Remains { get; set; }
    public Team Owner{get; set;}
    public NextEnemyHitDoubleFilter(int stacks = 1, Team owner = Team.Enemy)
    {
        Owner = owner;
        Remains = stacks;
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        // "적이 받는 다음 1회 데미지" = 플레이어가 적에게 주는 데미지
        if (e.type != ActionType.Damage) return;
        if (e.source == Owner) return;
        if (e.target != TargetPolicy.Opponent) return;

        e.value *= 2;

        Remains--;
        if (Remains <= 0)
            ctx.RemoveFilter(this);
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx) { }

    public Sprite ReturnSprite()
    {
        return Resources.Load<Sprite>("Sprites/PlusDamage");
    }
}