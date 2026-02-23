using System;
using UnityEngine;

public class WeakFilter : IEventFilter
{
    public int Priority => -50; 

    private int _remainingAttacks;
    private readonly float _mult; 
    private readonly Team _owner;  

    public WeakFilter(Team owner, int attacks, float mult)
    {
        _owner = owner;
        _remainingAttacks = Math.Max(1, attacks);
        _mult = Mathf.Clamp(mult, 0.05f, 1f);
    }

    public void BeforeExecute(ref ActionEvent e, ResolveContext ctx)
    {
        if (_remainingAttacks <= 0) return;

        if (e.type == ActionType.Damage && e.source == _owner)
        {
            int newVal = Mathf.FloorToInt(e.value * _mult);
            e.value = Mathf.Max(0, newVal);
        }
    }

    public void AfterExecute(ActionEvent executed, ResolveContext ctx)
    {
        if (_remainingAttacks <= 0) return;

        if (!executed.cancelled && executed.type == ActionType.Damage && executed.source == _owner)
        {
            _remainingAttacks--;
            if (_remainingAttacks <= 0) ctx.RemoveFilter(this);
        }
    }
}