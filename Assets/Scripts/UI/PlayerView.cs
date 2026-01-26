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

    void Start()
    {
        turnManager.OnTurnStart += RefreshTurn;
        player.healthChange += Refresh;
        player.CostChange += Refresh;
        if (player != null && player.Sprite != null)
            portrait.sprite = player.Sprite;
        portrait.sprite = player.sprite;
        PlayerHp.text = player.health.ToString();
        PlayerDef.text = player.defense.ToString();
        PlayerCost.text = player.cost.ToString();
        Turn.text = turnManager.TurnIndex.ToString();
        
        RefreshTurn(turnManager.TurnIndex);
        Refresh();
    }
    
    public void Refresh()
    {
        if (player == null) return;

        PlayerHp.text = $"{player.health}/{player.maxHealth}";
        PlayerDef.text = player.defense > 0 ? player.defense.ToString() : "";
        PlayerCost.text = $"{player.cost}/{player.maxCost}";
    }

    public void RefreshTurn(int turnindex)
    {
        Turn.text = turnindex.ToString();
    }
}