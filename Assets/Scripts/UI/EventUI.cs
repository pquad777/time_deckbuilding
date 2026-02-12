using System;
using UnityEngine;
using TMPro;

public class EventUI : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text desc;

    private Action _onLeave;

    
    public void OpenRandom(Action onLeave)
    {
        _onLeave = onLeave;

        BuildRandomEvent();
        gameObject.SetActive(true);
    }

    public void BuildRandomEvent()
    {
        if (title) title.text = "EVENT";
        if (desc) desc.text = "이벤트가 발생했다! 선택지를 고르자.";
        // TODO: 선택지 버튼 2~3개 생성/연결
    }

    public void ShowGameOver()
    {
        if (title) title.text = "GAME OVER";
        if (desc) desc.text = "패배했다.";

        
        _onLeave = null;
        gameObject.SetActive(true);
    }

    
    public void OnClickContinue()
    {
        gameObject.SetActive(false);
        _onLeave?.Invoke(); // ✅ 다음 전투 시작(CombatEndRouter.StartNextCombat)
    }
}