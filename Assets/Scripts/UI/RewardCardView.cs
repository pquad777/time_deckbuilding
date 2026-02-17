using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCardView : MonoBehaviour
{
    [SerializeField] private Image CardFrame; 
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;   // 선택
    [SerializeField] private Image artImage;      // 나중에 사용 (없으면 비워도 됨)

    public void Bind(CardDefinition def)
    {
        if (!def) return;

        if (nameText) nameText.text = def.displayName;

        // 지금 CardDefinition에 설명 필드 없을 수 있으니 임시 문자열
        if (descText) descText.text = $"Type: {def.Type} | Cost: {def.cost} | Power: {def.power} | Cast: {def.castTimeTurns}";

        // 나중에 def.artwork 같은 게 생기면 여기서 넣으면 됨
        // if (artImage) artImage.sprite = def.artwork;
    }
}