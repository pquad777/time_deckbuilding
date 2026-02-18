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

        if (descText)
            descText.text = CardTextBuilder.Build(def);

        // 아트는 나중
        // if (artImage) artImage.sprite = def.artwork;
    }
}