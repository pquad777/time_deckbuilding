using System;
using UnityEngine;

public class VulnerableFilter : IEventFilter
{
    public int Priority => -40; 
    public int Remains { get; set; }

    private readonly float _mult;
    public Team Owner { get; set; }

    public VulnerableFilter(Team victim, int hits, float mult)
    {
        Owner = victim;
        Remains = Math.Max(1, hits);
        _mult = Mathf.Max(1f, mult);
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        if (e.type != ActionType.Damage) return;

        bool targetsOpponent = e.target == TargetPolicy.Opponent;

        Team victimThisEvent = targetsOpponent
            ? (e.source == Team.Player ? Team.Enemy : Team.Player)
            : e.source; // Self면 source 자신

        if (victimThisEvent == Owner)
        {
            int newVal = Mathf.CeilToInt(e.value * _mult);
            e.value = Mathf.Max(0, newVal);
        }
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx)
    {
        if (Remains <= 0) return;

        if (executed.cancelled) return;
        if (executed.type != ActionType.Damage) return;

        bool targetsOpponent = executed.target == TargetPolicy.Opponent;
        Team victimThisEvent = targetsOpponent
            ? (executed.source == Team.Player ? Team.Enemy : Team.Player)
            : executed.source;

        if (victimThisEvent == Owner)
        {
            Remains--;
            if (Remains <= 0) ctx.RemoveFilter(this);
        }
    }    
    public Sprite ReturnSprite()
    {
        return Resources.Load<Sprite>("Sprites/Vulnerable");
    }
}