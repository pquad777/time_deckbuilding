using System;
using UnityEngine;

public class WeakFilter : IEventFilter
{
    public int Priority => -50; 
    public int Remains { get; set; }

    private readonly float _mult; 
    public Team Owner { get; set; }

    public WeakFilter(Team owner, int attacks, float mult)
    {
        Owner = owner;
        Remains = Math.Max(1, attacks);
        _mult = Mathf.Clamp(mult, 0.05f, 1f);
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        if (e.type == ActionType.Damage && e.source == Owner)
        {
            int newVal = Mathf.FloorToInt(e.value * _mult);
            e.value = Mathf.Max(0, newVal);
        }
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        if (!executed.cancelled && executed.type == ActionType.Damage && executed.source == Owner)
        {
            Remains--;
            if (Remains <= 0) ctx.RemoveFilter(this);
        }
    }
    public Sprite ReturnSprite()
    {
        return Resources.Load<Sprite>("Sprites/Weak");
    }
}