using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    [SerializeField] private CardView[] slots; // 길이 4 (0~3)
    private DeckSystem _deck;
    private Vector3 unselectedPos = Vector3.zero;
    private Vector3 selectedPos = Vector3.up * 10;
    
    public void Init(DeckSystem deck)
    {
        if (_deck != null) _deck.OnHandChanged -= Refresh;

        _deck = deck;
        _deck.OnHandChanged += Refresh;
        // 슬롯 인덱스 세팅
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSlotIndex(i);
        UnhighlightSlots();
        Refresh();
    }

    private void OnDestroy()
    {
        if (_deck != null) _deck.OnHandChanged -= Refresh;
    }

    private void Refresh()
    {
        for (int i = 0; i < 4; i++)
        {
            var card = _deck.GetCard(i);
            if (card == null) slots[i].Clear();
            else slots[i].Bind(card);
        }
    }

    public void HighlightSlot(int idx)
    {
       UnhighlightSlots();
       slots[idx].Highlight();
    }
    public void UnhighlightSlots()
    {
        for(int i=0;i<slots.Length;i++)
        {   
            slots[i].UnHighlight();
        }
    }
}
