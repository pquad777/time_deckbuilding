using System;
using UnityEngine;
using UnityEngine.UI;

public class RestUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    private Action _onLeave;

    public void Open(Action onLeave)
    {
        _onLeave = onLeave;
        gameObject.SetActive(true);
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            GameManager.instance.AudioManager.PlaySfx(AudioType.ClickButton);
            gameObject.SetActive(false);
            _onLeave?.Invoke();
        });
    }
}