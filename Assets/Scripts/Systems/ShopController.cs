using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private List<CardDefinition> cardPool;
    [SerializeField] private int offerCount = 5;
    
    [SerializeField] private int healCost = 100;
    [SerializeField] private int healAmount = 20;
    public IReadOnlyList<CardDefinition> CardPool => cardPool;
    [System.Serializable]
    public class Offer
    {
        public CardDefinition card;
        public int price;
        public bool sold;
    }

    
    public IReadOnlyList<Offer> Offers => _offers;
    private readonly List<Offer> _offers = new();

    public int HealCost => healCost;
    public int HealAmount => healAmount;
    private bool _healSold;
    public bool HealSold => _healSold;

    public void BuildShop()
    {
        _offers.Clear();
        _healSold = false;
        
        if (cardPool == null || cardPool.Count == 0)
        {
            Debug.LogWarning("ShopController: cardPool이 비어있음");
            return;
        }

        for (int i = 0; i < offerCount; i++)
        {
            var card = cardPool[Random.Range(0, cardPool.Count)];
            _offers.Add(new Offer
            {
                card = card,
                price = GetPrice(card),
                sold = false
            });
        }
    }

    public bool TryBuyCard(int idx)
    {
        if (idx < 0 || idx >= _offers.Count) return false;

        var offer = _offers[idx];
        if (offer.sold) return false;
        if (!player.TrySpend(offer.price)) return false;

        player.AddCard(offer.card);
        offer.sold = true;
        return true;
    }

    
    public bool TryBuyHeal()
    {
        if (_healSold) return false;               
        bool ok = player.TryHeal(healAmount, healCost);
        if (!ok) return false;

        _healSold = true;                          
        return true;
    }

    private int GetPrice(CardDefinition card)
    {
        return 50; // TODO: rarity 기반으로 확장
    }

    
}