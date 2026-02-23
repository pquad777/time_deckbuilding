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

    [Header("Offer List UI")]
    [SerializeField] private Transform offerParent;      
    [SerializeField] private CardView offerPrefab;    
 

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
        RebuildOffers();
    }

    private void RefreshHeader()
    {
        if (goldText) goldText.text = $"{player.gold}C";
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
            ShopOfferItem offerItem = item.GetComponentInChildren<ShopOfferItem>();
            var button = Instantiate(offerItem, item.transform);
            item.Bind(new CardInstance(offer.card));
            bool canAfford = player.gold >= offer.price;
            button.Bind(
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
