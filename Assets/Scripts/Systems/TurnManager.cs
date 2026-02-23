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
    private bool isRunning = false;
    private bool isCounting = true;
    private void Update()
    {
        if (!isRunning) return;
        elapsedTime += Time.deltaTime;
        if (elapsedTime < turnDurationSeconds) return;
        
        elapsedTime = 0f;
        
        OnTurnEnd?.Invoke(TurnIndex);
        if (!isRunning) return;
        if(isCounting) TurnIndex++;
        
        OnTurnStart?.Invoke(TurnIndex);
        //1.5초마다 턴엔드, 턴 스타트 신호를 보냄
    }

    public void StartLoop(bool resetTurnIndex, bool isCounting = true)
    {
        this.isCounting = isCounting;
        if (resetTurnIndex) TurnIndex = 0;
        isRunning = true;
        OnTurnStart?.Invoke(TurnIndex);
    }

    public void EndLoop()
    {
        isRunning = false;
        elapsedTime = 0f;
    }
}
