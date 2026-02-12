using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardButton : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button button;

    public void Bind(string cardName, int price, bool sold, Action onClick)
    {
        if (nameText) nameText.text = cardName;
        if (priceText) priceText.text = sold ? "SOLD" : $"{price} G";

        button.onClick.RemoveAllListeners();
        button.interactable = !sold;
        button.onClick.AddListener(() => onClick());
    }
}