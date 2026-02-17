using System;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ShopController shop;

    [Header("Header UI")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text hpText;

    [Header("Offer List UI")]
    [SerializeField] private Transform offerParent;      
    [SerializeField] private ShopOfferItem offerPrefab;  

    [Header("Heal UI")]
    [SerializeField] private UnityEngine.UI.Button healButton;
    [SerializeField] private TMP_Text healText; 

    private Action _onLeave;
    
    public void Open(Action onLeave)
    {
        _onLeave = onLeave;
        gameObject.SetActive(true);

        Debug.Log($"ShopUI Open. shop null? {shop == null}, player null? {player == null}");
        shop.BuildShop();
        Debug.Log($"Offers count: {shop.Offers.Count}");
        RefreshAll();
    }

    private void RefreshAll()
    {
        RefreshHeader();
        RefreshHeal();
        RebuildOffers();
    }

    private void RefreshHeader()
    {
        if (goldText) goldText.text = $"{player.gold}G";
        if (hpText) hpText.text = $"{player.health}/{player.maxHealth}";
    }

    private void RefreshHeal()
    {
        bool canHeal = !shop.HealSold
                       && player.health < player.maxHealth
                       && player.gold >= shop.HealCost;

        if (healButton) healButton.interactable = canHeal;

        if (!healText) return;
        if (shop.HealSold) healText.text = "SOLD";
        healText.text = $"Heal +{shop.HealAmount} ({shop.HealCost}G)";
    }

    private void RebuildOffers()
    {
        for (int i = offerParent.childCount - 1; i >= 0; i--)
            Destroy(offerParent.GetChild(i).gameObject);

        var offers = shop.Offers;
        for (int i = 0; i < offers.Count; i++)
        {
            int idx = i;
            var offer = offers[i];

            var item = Instantiate(offerPrefab, offerParent);

            bool canAfford = player.gold >= offer.price;
            item.Bind(
                cardName: offer.card.displayName, // displayName 쓰는 게 더 자연스러움
                price: offer.price,
                sold: offer.sold,
                canAfford: canAfford,
                onBuy: () =>
                {
                    shop.TryBuyCard(idx);
                    RefreshAll();
                }
            );
        }
    }

    
    public void OnClickHeal()
    {
        if (shop.TryBuyHeal())
            RefreshAll();
        else
            return;
    }

    
    public void OnClickLeave()
    {
        gameObject.SetActive(false);
        _onLeave?.Invoke();
    }
}
