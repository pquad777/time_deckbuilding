using UnityEngine;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    [SerializeField] private CardView[] slots; // 길이 4 (0~3)
    private DeckSystem _deck;

    public void Init(DeckSystem deck)
    {
        if (_deck != null) _deck.OnHandChanged -= Refresh;

        _deck = deck;
        _deck.OnHandChanged += Refresh;

        // 슬롯 인덱스 세팅
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSlotIndex(i);

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
    
}
