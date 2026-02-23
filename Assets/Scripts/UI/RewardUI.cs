using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RewardUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ShopController shop; 
    
    [Header("UI")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private CardView cardView;

    [SerializeField] private Button acceptButton;
    [SerializeField] private Button leaveButton;

    private Action _onLeave;
    private CardDefinition _offered;
    private bool _decided;

    public void Open(Action onLeave)
    {
        _onLeave = onLeave;
        gameObject.SetActive(true);

        BuildReward();
        RefreshUI();
    }

    private void BuildReward()
    {
        _decided = false;

        // 1) 남은 체력만큼 골드 지급
        int gain = Mathf.Max(0, player.health);
        player.gold += gain;

        // 2) ShopController의 cardPool에서 랜덤 1장
        _offered = PickRandomCardFromShopPool();
    }
    private CardDefinition PickRandomCardFromShopPool()
    {

        var pool = shop != null ? shop.CardPool : null;
        if (pool == null || pool.Count == 0) return null;

        return pool[Random.Range(0, pool.Count)];
    }
    
    private void RefreshUI()
    {
        if (goldText) goldText.text = $"+{Mathf.Max(0, player.health)}G  (Gold: {player.gold})";
        if (hpText) hpText.text = $"HP: {player.health}/{player.maxHealth}";

        if (cardView&& _offered)
        {
            cardView.Bind(new CardInstance(_offered));
            cardView.UnHighlight();
        }

        bool canChoose = !_decided && _offered != null;
        if (acceptButton) acceptButton.interactable = canChoose;
    }

    
    public void OnClickAccept()
    {
        if (_decided || !_offered) return;

        player.AddCard(_offered);
        _decided = true;
        RefreshUI();
    }
   
    
    public void OnClickLeave()
    {
        gameObject.SetActive(false);
        _onLeave?.Invoke();
    }
}