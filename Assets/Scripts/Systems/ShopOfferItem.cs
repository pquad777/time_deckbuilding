using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopOfferItem : MonoBehaviour
{
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    public void Bind(string cardName, int price, bool sold, bool canAfford, Action onBuy)
    {
        cardNameText.text = cardName;
        priceText.text = sold ? "SOLD" : $"{price}G";

        buyButton.onClick.RemoveAllListeners();
        buyButton.interactable = !sold && canAfford;

        buyButton.onClick.AddListener(() => onBuy());
    }
}