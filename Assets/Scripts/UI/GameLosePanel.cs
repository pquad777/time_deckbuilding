using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLosePanel : MonoBehaviour
{
    [SerializeField] private Button toStartButton;

    void Awake()
    {
        if (toStartButton)
        {
            toStartButton.onClick.RemoveAllListeners();
            toStartButton.onClick.AddListener(ToStart);
        }
    }

    private void ToStart()
    {
        GameFlowManager.I.SetState(GameState.StartMenu);
    }
}