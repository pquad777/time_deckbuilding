using UnityEngine;
using TMPro;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] private CombatController combatController;
    [SerializeField] private TMP_Text resultText;

    private void Awake()
    {
        gameObject.SetActive(false); // 시작 시 숨김
    }

    private void OnEnable()
    {
        combatController.OnCombatEnded += HandleCombatEnded;
    }

    private void OnDisable()
    {
        combatController.OnCombatEnded -= HandleCombatEnded;
    }

    private void HandleCombatEnded(CombatController.CombatResult result)
    {
        gameObject.SetActive(true);
        resultText.text = result == CombatController.CombatResult.Win ? "VICTORY" : "DEFEAT";
    }
}