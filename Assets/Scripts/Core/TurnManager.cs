using System;
using UnityEngine;

public class TurnManager:MonoBehaviour
{
    [Header("Turn Settings")]
    [SerializeField] private float turnDurationSeconds = 1.5f;
    
    public event Action<int> OnTurnStart; // turnIndex
    public event Action<int> OnTurnEnd;   // turnIndex
    
    public int TurnIndex { get; private set; } = 0;
    
    private float elapsedTime = 0f;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < turnDurationSeconds) return;
        
        elapsedTime = 0f;
        
        OnTurnEnd?.Invoke(TurnIndex);
        TurnIndex++;
        OnTurnStart?.Invoke(TurnIndex);
    }

    public void StartLoop(bool resetTurnIndex)
    {
        if (resetTurnIndex) TurnIndex = 0;
        
        OnTurnStart?.Invoke(TurnIndex);
    }

    public void EndLoop()
    {
        elapsedTime = 0f;
    }
}
