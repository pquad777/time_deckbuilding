using UnityEngine;
using System;
using System.Collections.Generic;

public class CombatController:MonoBehaviour
{
    private TurnManager _turnManager;
    private InputController _inputController;

    private int _handSize = 4;
    private int _defaultDiscardIndex = 3;

    private List<CardDefinition> _debugPlayerDeck = new();
    private DeckSystem _deckSystem;

    private void Awake()
    {
        _turnManager.OnTurnStart += TurnStart;
        _turnManager.OnTurnEnd += TurnEnd;
    }

    private void OnDestroy()
    {
        _turnManager.OnTurnStart -= TurnStart;
        _turnManager.OnTurnEnd -= TurnEnd;
        
    }

    private void Start()
    {
        StartCombat(_debugPlayerDeck);
    }

    public void StartCombat(IReadOnlyList<CardDefinition> playerdeck)
    {
        _deckSystem = new DeckSystem(_handSize);
        var rng = new System.Random(); //시드 고정 가능(ex) new System.Random(1234)
        _deckSystem.Init(playerdeck, rng);
        _deckSystem.InitHand(_handSize);
        
        PrintHand("Combat Start Hand");
        
        _inputController.Enable(true);
        _turnManager.StartLoop(true);
    }

    private void TurnStart(int turnIndex)
    {
        _inputController.Enable(true);
        _inputController.ClearChoice();
        
        Debug.Log($"== TURN {turnIndex} START ==");
        PrintHand("Hand");
    }

    private void TurnEnd(int turnIndex)
    {
        Debug.Log($"-- TURN {turnIndex} END (Resolve) --");
        _inputController.Enable(false);
        ResolveTurn();
        PrintHand("AfterResolve");
    }

    private void ResolveTurn()
    {
        if (_inputController.ChosenIndex.HasValue)
        {
            int idx = _inputController.ChosenIndex.Value;
            var card = _deckSystem.PlayFromHand(idx);
            Debug.Log($"PLAY: {card.Def.displayName} ({card.Def.type}, value={card.Def.value})");
        }
        else DiscardDefault();
        
        _deckSystem.DrawToHand();
        _inputController.ClearChoice();
        
    }

    private void DiscardDefault()
    {
        int idx = Math.Clamp(_defaultDiscardIndex, 0, _deckSystem.HandCount - 1);
        var card = _deckSystem.DiscardFromHand(idx);
        Debug.Log($"DISCARD(default): {card.Def.displayName}");
    }
    
    private void PrintHand(string label)
    {
        if (_deckSystem == null) return;

        string s = $"{label} [";
        for (int i = 0; i < _deckSystem.HandCount; i++)
        {
            s += (i == 0 ? "" : ", ") + _deckSystem.Hand[i].Def.displayName;
        }
        s += "]";
        Debug.Log(s);
    }
}
