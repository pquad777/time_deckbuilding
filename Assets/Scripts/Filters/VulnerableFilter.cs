using System;
using UnityEngine;

public class VulnerableFilter : IEventFilter
{
    public int Priority => -40; 

    private int _remainingHits;
    private readonly float _mult; 
    private readonly Team _victim; 

    public VulnerableFilter(Team victim, int hits, float mult)
    {
        _victim = victim;
        _remainingHits = Math.Max(1, hits);
        _mult = Mathf.Max(1f, mult);
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (_remainingHits <= 0) return;

        if (e.type != ActionType.Damage) return;

        bool targetsOpponent = e.target == TargetPolicy.Opponent;

        Team victimThisEvent = targetsOpponent
            ? (e.source == Team.Player ? Team.Enemy : Team.Player)
            : e.source; // Self면 source 자신

        if (victimThisEvent == _victim)
        {
            int newVal = Mathf.CeilToInt(e.value * _mult);
            e.value = Mathf.Max(0, newVal);
        }
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx)
    {
        if (_remainingHits <= 0) return;

        if (executed.cancelled) return;
        if (executed.type != ActionType.Damage) return;

        bool targetsOpponent = executed.target == TargetPolicy.Opponent;
        Team victimThisEvent = targetsOpponent
            ? (executed.source == Team.Player ? Team.Enemy : Team.Player)
            : executed.source;

        if (victimThisEvent == _victim)
        {
            _remainingHits--;
            if (_remainingHits <= 0) ctx.RemoveFilter(this);
        }
    }
}