using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image art;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image highlightImage;
    [SerializeField] private Image DisplayDisabledImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text powerText;
    
    public int _slotIndex;
    public void SetSlotIndex(int slotIndex) => _slotIndex = slotIndex;
    
    

    
    public void Bind(CardInstance card)
    {
        var def = card.def;
        frameImage.sprite = def.frame;
        frameImage.enabled = true;
        
        art.enabled = true;
        art.sprite = def.artwork;
        nameText.text = def.displayName;
        costText.text = def.cost.ToString();
        powerText.text = CardTextBuilder.Build(def);
        gameObject.SetActive(true);

    }
    
    public void Clear()
    {
        frameImage.sprite = null;
        frameImage.enabled = false;
        art.sprite = null;
        art.enabled = false;
        nameText.text = "";
        costText.text = "";
        powerText.text = "";
        gameObject.SetActive(false);
    }

    public void Highlight()
    {
        highlightImage.enabled = true;
    }
    public void UnHighlight()
    {
        highlightImage.enabled = false;
    }

    public void DisplayDisabled()
    {
        DisplayDisabledImage.enabled = true;   
    }

    public void DisplayEnabled()
    {
        DisplayDisabledImage.enabled = false;
    }
}
