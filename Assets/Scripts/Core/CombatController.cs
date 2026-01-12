using UnityEngine;
using System;
using System.Collections.Generic;

public class CombatController:MonoBehaviour
{
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private InputController _inputController;

    private int _handSize = 4;
    private int _defaultDiscardIndex = 3;

    [SerializeField]private List<CardDefinition> _debugPlayerDeck = new();
    private DeckSystem _deckSystem;

    public class CombatState // 시뮬 용으로 간단하게 구성
    {
        public int playerHp = 300;
        public int enemyHp = 30;
        public int playercost = 10;
        public int enemyDamage = 2;
        public int playerDefense = 0;
    }
    private CombatState _combatState = new CombatState();
    private void Awake()
    {
        _turnManager.OnTurnStart += TurnStart;
        _turnManager.OnTurnEnd += TurnEnd;
        
        _inputController.OnCardKeyPressed += HandleCardKeyPressed;
        _inputController.OnCancelPressed += HandleCancelPressed;
    }
    

    private void OnDestroy()
    {
        _turnManager.OnTurnStart -= TurnStart;
        _turnManager.OnTurnEnd -= TurnEnd;
        
        _inputController.OnCardKeyPressed += HandleCardKeyPressed;
        _inputController.OnCancelPressed += HandleCancelPressed;
    }

    private void Start()
    {
        StartCombat(_debugPlayerDeck);
    }
    
    public void StartCombat(IReadOnlyList<CardDefinition> playerdeck)
    {
        _deckSystem = new DeckSystem(_handSize); //이 부분 확인 필요
        var rng = new System.Random(); //시드 고정 가능(ex) new System.Random(1234)
        _deckSystem.Init(playerdeck, rng);
        _deckSystem.InitHand(_handSize);
        _combatState = new CombatState();
        
        if (playerdeck == null || playerdeck.Count == 0) Debug.LogError("Deck is empty!");

        
        PrintHand("Combat Start Hand");
        
        
        _turnManager.StartLoop(true);
    }

    private void TurnStart(int turnIndex)
    {
        _inputController.Enable(true);
        _inputController.ClearChoice();
        _combatState.playerDefense = 0; // 추후 수정 예정(현재는 duration 적용 못함, 턴마다 디펜스 초기화)
        
        Debug.Log($"== TURN {turnIndex} START ==");
        PrintHand("Hand");
        Debug.Log($"Current Cost : {_combatState.playercost}");
    }

    private void TurnEnd(int turnIndex)
    {
        Debug.Log($"-- TURN {turnIndex} END (Resolve) --");
        _inputController.Enable(false);
        ResolveTurn();
        EnemyAttack();
        CheckEnd();
        PrintHand("AfterResolve");
        PrintState();
    }

    private void ResolveTurn()
    {
        if (_inputController.ChosenIndex.HasValue)
        {
            int idx = _inputController.ChosenIndex.Value;
            var card = _deckSystem.PlayFromHand(idx);
            var def = card.Def;
            _combatState.playercost -= def.cost;
            switch (def.Type)
            {
                case CardType.Attack:
                    _combatState.enemyHp -= def.power;
                    break;

                case CardType.Defense:
                    _combatState.playerDefense += def.power;
                    break;

                case CardType.Dodge:
                    break;//제작 예정
            }
        }
        else
        {
            DiscardDefault();
        }
        _deckSystem.DrawToHand();
        _inputController.ClearChoice();
        
    }
    

    private void DiscardDefault()
    {
        int idx = Math.Clamp(_defaultDiscardIndex, 0, _deckSystem.HandCount - 1);
        var card = _deckSystem.DiscardFromHand(idx);
        _combatState.playercost = Math.Min(10, ++_combatState.playercost);
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

    public bool CanUseCard(int idx)
    {
        var card = _deckSystem.Hand[idx];
        var def = card.Def;
        return _combatState.playercost >= def.cost;
    }

    private void EnemyAttack()
    {
        int dmg = _combatState.enemyDamage;
        int net = Math.Max(0, dmg - _combatState.playerDefense);
        _combatState.playerHp -= net;
    }
    private void CheckEnd() 
    {
        if (_combatState.enemyHp <= 0)
        {
            Debug.Log("WIN");
            _turnManager.EndLoop();
        }
        else if (_combatState.playerHp <= 0)
        {
            Debug.Log("LOSE");
            _turnManager.EndLoop();
        }
    }

    private void PrintState()
    {
        Debug.Log($"HP: {_combatState.playerHp} Enemy:{_combatState.enemyHp}");
    }
    private void HandleCancelPressed()
    {
        _inputController.ClearChoice();
    }
    
    private void HandleCardKeyPressed(int idx)
    {
        // 코스트 체크
        if (!CanUseCard(idx))
        {
            Debug.Log("Not enough cost");
            return;
        }

        _inputController.SetChoice(idx); // 선택 확정
    }


}
