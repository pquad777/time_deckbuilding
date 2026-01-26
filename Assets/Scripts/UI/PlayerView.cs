using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text PlayerHp;
    [SerializeField] private TMP_Text PlayerDef;
    

    void Start()
    {
        player.healthChange += Refresh;
        if (player != null && player.Sprite != null)
            portrait.sprite = player.Sprite;
        portrait.sprite = player.sprite;
        PlayerHp.text = player.health.ToString();
        PlayerDef.text = player.defense.ToString();
        
        Refresh();
    }
    
    public void Refresh()
    {
        if (player == null) return;

        PlayerHp.text = $"{player.health}/{player.maxHealth}";
        PlayerDef.text = player.defense > 0 ? player.defense.ToString() : "";
    }
}