using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopOfferItem : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    public void Bind( int price, bool sold, bool canAfford, Action onBuy)
    {
        priceText.text = sold ? "품절" : $"{price}C";

        buyButton.onClick.RemoveAllListeners();
        buyButton.interactable = !sold && canAfford;

        buyButton.onClick.AddListener(() => onBuy());
    }
}