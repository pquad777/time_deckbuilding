using UnityEngine;
using System;
using System.Collections.Generic;
using Random = Unity.Mathematics.Random;

public class DeckSystem
{
    private Queue<CardInstance> _deckQueue = new();
    private List<CardInstance> _hand;
    
    public IReadOnlyList<CardInstance> Hand => _hand;
    public int DeckCount => _deckQueue.Count;
    public int HandCount => _hand.Count;
    public event Action OnHandChanged;

    public DeckSystem(int initalHandCapacity = 4)
    {
        _hand = new List<CardInstance>(initalHandCapacity);
    }
    
    //전투 시작 시 호출, 셔플하고 큐 채우기, 
    public void Init(IReadOnlyList<CardDefinition> playerDeck, System.Random? rng = null)
    {
        _deckQueue.Clear();
        _hand.Clear();
        
        var temp = new List<CardInstance>(playerDeck.Count);
        for (int i = 0; i < playerDeck.Count; i++) temp.Add(new CardInstance(playerDeck[i]));
        
        Shuffle(temp, rng ?? new System.Random());
        
        for(int i =0; i<temp.Count; i++) _deckQueue.Enqueue(temp[i]);
    }
    
    //최초 손패 뽑기
    public void InitHand(int handSize)
    {
        _hand.Clear();
        for (int i = 0; i < handSize-1; i++) DrawToHand();
    }
    
    //한장씩 드로우
    public void DrawToHand()
    {
        var card =  _deckQueue.Dequeue();
        _hand.Add(card);
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
    public CardInstance PlayFromHand(int index)
    {
        return RemoveFromHandToBottom(index);
    }

    public CardInstance DiscardFromHand(int index)
    {
        return RemoveFromHandToBottom(index);
    }
    //카드 사용하고 덱 맨 밑장으로 순환시키기
    private CardInstance RemoveFromHandToBottom(int index) 
    {
        var card = _hand[index];
        _hand.RemoveAt(index);
        _deckQueue.Enqueue(card);

        OnHandChanged?.Invoke(); //패에 변화가 있다는 신호
        return card;
    }
    


}



//추후 카드가 전투 중 성능이 변하는 방향으로 나아갈 때 활용해야함(지금은 큰 의미 없음)
public sealed class CardInstance
{
    public CardDefinition Def { get; }

    public CardInstance(CardDefinition def)
    {
        Def = def;
    }
}
