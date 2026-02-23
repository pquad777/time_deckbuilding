using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text PlayerHp;
    [SerializeField] private TMP_Text PlayerDef;
    [SerializeField] private TMP_Text PlayerCost;
    [SerializeField] private TMP_Text Turn;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private GameObject Def;
    private float HpBarOriginSize;
    void Start()
    {
        turnManager.OnTurnStart += RefreshTurn;
        player.OnPlayerDataChanged += Refresh;
        if (player != null && player.Sprite != null)
            portrait.sprite = player.Sprite;
        portrait.sprite = player.sprite;
        PlayerHp.text = $"HP: {player.health}/{player.maxHealth}"; 
        HpBarOriginSize = hpBar.rect.width;
        Def.SetActive(player.defense > 0);
        PlayerDef.text = player.defense > 0 ? $"DEF: {player.defense}" : "";
        PlayerCost.text = $"COST: {player.cost}/{player.maxCost}";
        Turn.text = $"TURN: {turnManager.TurnIndex}";
        
        RefreshTurn(turnManager.TurnIndex);
        Refresh();
    }
    
    public void Refresh()
    {
        if (player == null) return;
        PlayerHp.text = $"{player.health}/{player.maxHealth}";
        hpBar.sizeDelta = new Vector2(HpBarOriginSize * player.health/player.maxHealth, hpBar.sizeDelta.y);
        hpBar.anchoredPosition = new Vector2(-(HpBarOriginSize-hpBar.sizeDelta.x)/2, hpBar.anchoredPosition.y);
        Def.SetActive(player.defense > 0);
        PlayerDef.text = player.defense > 0 ? $"{player.defense}" : "";
        PlayerCost.text = $"{player.cost}/{player.maxCost}";
    }

    public void RefreshTurn(int turnindex)
    {
        Turn.text = $"{turnindex}";
    }
}