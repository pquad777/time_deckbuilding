using UnityEngine;
using System;
using System.Collections.Generic;
using Random = Unity.Mathematics.Random;

public class DeckSystem
{
    private Queue<CardInstance> _deckQueue = new();
    private CardInstance[] _hand;
    
    public IReadOnlyList<CardInstance> Hand => _hand;
    public int HandCount => _hand.Length;
    public event Action OnHandChanged;

    public DeckSystem(int initalHandCapacity = 4)
    {
        _hand = new CardInstance[initalHandCapacity];
    }

    public CardInstance GetCard(int slotIndex) => _hand[slotIndex];
    public bool HasCard(int slotIndex) => _hand[slotIndex] != null;
    //전투 시작 시 호출, 셔플하고 큐 채우기, 
    public void Init(IReadOnlyList<CardDefinition> playerDeck, System.Random? rng = null)
    {
        _deckQueue.Clear();
        Array.Clear(_hand, 0,_hand.Length);

        var temp = new List<CardInstance>(playerDeck.Count);
        for (int i = 0; i < playerDeck.Count; i++) temp.Add(new CardInstance(playerDeck[i]));

        Shuffle(temp, rng ?? new System.Random());
        for (int i = 0; i < temp.Count; i++) _deckQueue.Enqueue(temp[i]);

        OnHandChanged?.Invoke();
    }
    
    //최초 손패 뽑기
    public void InitHand(int handSize)
    {
        Array.Clear(_hand, 0, _hand.Length);
        for (int i = 0; i < handSize; i++) DrawToHand();
        CompactRight();
        OnHandChanged?.Invoke();
    }
    
    //한장씩 드로우
    public void DrawToHand()
    {
        int empty = FindLeftmostEmptySlot();
        if (empty == -1) return;              
        if (_deckQueue.Count == 0) return;  
        CompactRight();
        _hand[0] = _deckQueue.Dequeue();
        
        OnHandChanged?.Invoke();
    }
    
    //셔플
    private static void Shuffle(List<CardInstance> list, System.Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--) //Fisher-Yates
        {
            int j =rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    //손에서 카드 사용
    public CardInstance PlayFromHand(int index) => RemoveFromHandToBottom(index);
    

    public CardInstance DiscardFromHand(int index) => RemoveFromHandToBottom(index);
    //카드 사용하고 덱 맨 밑장으로 순환시키기
    private CardInstance RemoveFromHandToBottom(int slotIndex) 
    {
        var card = _hand[slotIndex];
        if (card == null) throw new InvalidOperationException($"Slot {slotIndex} is empty.");

        _hand[slotIndex] = null;
        _deckQueue.Enqueue(card);

        CompactRight();
        OnHandChanged?.Invoke();
        return card;
    }
    private void CompactRight()
    {
        for (int i = _hand.Length - 1; i >= 0; i--)
        {
            if (_hand[i] != null) continue;

            int j = i - 1;
            while (j >= 0 && _hand[j] == null) j--;

            if (j < 0) break; // 왼쪽에 더 이상 카드 없음
            _hand[i] = _hand[j];
            _hand[j] = null;
        }
    }

    private int FindLeftmostEmptySlot()
    {
        for (int i = 0; i < _hand.Length; i++)
            if (_hand[i] == null) return i;
        return -1;
    }


}



//추후 카드가 전투 중 성능이 변하는 방향으로 나아갈 때 활용해야함(지금은 큰 의미 없음)
public sealed class CardInstance
{
    public CardDefinition def;
    
    public CardInstance(CardDefinition def)
    {
        this.def = def;
    }
}
