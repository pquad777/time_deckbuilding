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

    [Header("Card Pool (reward)")]
    [SerializeField] private List<CardDefinition> rewardCardPool;

    [Header("UI")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardDescText;

    [SerializeField] private Button acceptButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button leaveButton;

    private Action _onLeave;
    private CardDefinition _offeredCard;
    private bool _choiceLocked;

    public void Open(Action onLeave)
    {
        _onLeave = onLeave;
        gameObject.SetActive(true);

        BuildReward();
        RefreshUI();
    }

    private void BuildReward()
    {
        _choiceLocked = false;
        _offeredCard = null;

        // 1) 남은 체력만큼 골드 지급
        int gain = Mathf.Max(0, player.health);
        player.gold += gain;

        // 2) 랜덤 카드 1장 제시
        if (rewardCardPool != null && rewardCardPool.Count > 0)
        {
            _offeredCard = rewardCardPool[Random.Range(0, rewardCardPool.Count)];
        }

        // 버튼 상태 초기화
        if (acceptButton) acceptButton.interactable = (_offeredCard != null);
        if (skipButton) skipButton.interactable = true;
        if (leaveButton) leaveButton.interactable = true;
    }

    private void RefreshUI()
    {
        if (goldText) goldText.text = $"+{Mathf.Max(0, player.health)}G  (현재 골드: {player.gold}G)";
        if (hpText) hpText.text = $"HP: {player.health}/{player.maxHealth}";

        if (_offeredCard == null)
        {
            if (cardNameText) cardNameText.text = "No Card";
            if (cardDescText) cardDescText.text = "보상 카드 풀이 비어있습니다.";
            return;
        }

        if (cardNameText) cardNameText.text = _offeredCard.displayName;
        if (cardDescText)
        {
            // CardDefinition에 설명 필드가 없으면 일단 타입/코스트/파워로 표시
            cardDescText.text = $"Type: {_offeredCard.Type} | Cost: {_offeredCard.cost} | Power: {_offeredCard.power}";
        }
    }

    // 버튼 연결: “받기”
    public void OnClickAccept()
    {
        if (_choiceLocked) return;
        if (_offeredCard == null) return;

        player.AddCard(_offeredCard);
        LockChoice("카드 획득 완료!");
    }

    // 버튼 연결: “거절”
    public void OnClickSkip()
    {
        if (_choiceLocked) return;
        LockChoice("카드 스킵");
    }

    private void LockChoice(string msg)
    {
        _choiceLocked = true;

        if (acceptButton) acceptButton.interactable = false;
        if (skipButton) skipButton.interactable = false;

        if (cardDescText) cardDescText.text += $"\n\n[{msg}]";
    }

    // 버튼 연결: “Leave”
    public void OnClickLeave()
    {
        gameObject.SetActive(false);
        _onLeave?.Invoke();
    }
}