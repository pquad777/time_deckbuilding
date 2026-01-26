using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CombatController : MonoBehaviour
{
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private InputController _inputController;
    [SerializeField] private HandUI handUI;
    [SerializeField] private EnemySlotView enemyView;
    [SerializeField] private GameManager gameManager;
    private int _handSize = 4;
   
    private PlayerController _playerController;
    private EnemyController _enemyController = new();
    private DeckSystem _deckSystem;
    private System.Random _aiRng = new System.Random();
    
    

    private void Start()
    {
        StartCombat(gameManager.RandomEnemyEncounter());
    }

    public void StartCombat(EnemyDefinition enemyDefinition)
    {
        
        _enemyController.LoadInfo(enemyDefinition);
        enemyView.Bind(_enemyController);
        _playerController = GameManager.instance.playerController;
        _deckSystem = new DeckSystem(_handSize); 
        var rng = new System.Random();
        _deckSystem.Init(_playerController.playerDeck, rng);
        _deckSystem.InitHand(_handSize);
        handUI.Init(_deckSystem);
        
        _turnManager.OnTurnStart += TurnStart;
        _turnManager.OnTurnEnd += TurnEnd;

        _inputController.OnCardKeyPressed += HandleCardKeyPressed;
        _inputController.OnCancelPressed += HandleCancelPressed;


        _turnManager.StartLoop(true);
        PrintHand("Combat Start Hand");
    }

    private void TurnStart(int turnIndex)
    {
        Debug.Log($"== TURN {turnIndex} START ==");

        EnemyStartCast();
        _playerController.EndDodge();
        if(_deckSystem.HandCount == 4)DiscardDefault();
        _deckSystem.DrawToHand();

        bool canAct = !_playerController.isCasting;
        _inputController.Enable(canAct);
        HandleCancelPressed();

        PrintHand("Hand");
        Debug.Log($"Current Cost : {_playerController.cost}");
        if (!canAct)
            Debug.Log($"CASTING... ({_playerController.remainCastTime} turns left)");

    }

    private void TurnEnd(int turnIndex)
    {
        Debug.Log($"-- TURN {turnIndex} END (Resolve) --");
        _inputController.Enable(false);
        ResolveTurn();


        CheckEnd();

        _playerController.health--;
        _playerController.healthChange.Invoke();
        PrintHand("AfterResolve");
        PrintState();

    }

    private void ResolveTurn()
    {
        
            Battle();
            HandleCancelPressed();
        _playerController.cost = Math.Min(_playerController.maxCost, ++_playerController.cost);


    }


    private void DiscardDefault()
    {
        int idx = 3;
        var card = _deckSystem.DiscardFromHand(idx);
        Debug.Log($"DISCARD(default): {card.def.displayName}");
    }

    private void PrintHand(string label)
    {
        if (_deckSystem == null) return;

        string s = $"{label} [";
        for (int i = _deckSystem.HandCount-1; i >= 0; i--)
        {
            var c = _deckSystem.GetCard(i);
            s += (i == 0 ? "" : ", ") + (c != null ? c.def.displayName : "_");
        }

        s += "]";
        Debug.Log(s);
    }

    public bool CanUseCard(int idx)
    {
        var card = _deckSystem.Hand[idx];
        var def = card.def;
        return _playerController.cost >= def.cost;
    }

    private void CheckEnd()
    {
        if (_enemyController.health <= 0)
        {
            Debug.Log("WIN");
            _turnManager.OnTurnStart -= TurnStart;
            _turnManager.OnTurnEnd -= TurnEnd;

            _inputController.OnCardKeyPressed -= HandleCardKeyPressed;
            _inputController.OnCancelPressed -= HandleCancelPressed;
            _turnManager.EndLoop();
        }
        else if (_enemyController.health <= 0)
        {
            Debug.Log("LOSE");
            _turnManager.OnTurnStart -= TurnStart;
            _turnManager.OnTurnEnd -= TurnEnd;

            _inputController.OnCardKeyPressed -= HandleCardKeyPressed;
            _inputController.OnCancelPressed -= HandleCancelPressed;
            _turnManager.EndLoop();
        }
    }

    private void PrintState()
    {
        Debug.Log(
            $"HP: {_playerController.health} Def:{_playerController.defense} Enemy:{_enemyController.health} EnemyDef:{_enemyController.defense}");
    }

    private void HandleCancelPressed()
    {
        _inputController.ClearChoice();
        handUI.UnhighlightSlots();
    }

    private void HandleCardKeyPressed(int idx)
    {
        if (_playerController.isCasting) return;
        if (!CanUseCard(idx)) return;
        handUI.HighlightSlot(idx);
        _inputController.SetChoice(idx); // 선택 확정
    }

    private void Battle()
    {
        if (_inputController.ChosenIndex.HasValue && !_playerController.isCasting)
        {
            int idx = _inputController.ChosenIndex.Value;
            var card = _deckSystem.PlayFromHand(idx);
            var def = card.def;
            _playerController.cost -= def.cost;
            StartCasting(def);
        }
        else if (_playerController.isCasting)
            ProcessPlayerCasting();
        else 
        Debug.Log("Done nothing.");

        ProcessEnemyCasting();

    }

    public void StartCasting(CardDefinition def)
    {

        if (_playerController.isCasting) return;
        if (def.castTimeTurns <= 0)
        {
            ProcessPlayerCard(def);
            return;
        }

        _playerController.StartCasting(def);
    }

    private void ProcessPlayerCard(CardDefinition cardDefinition)
    {
        CardDefinition def = cardDefinition;
        Debug.Log($"PLAYER FINISH CAST: type={def.Type}, turns={def.castTimeTurns}, value={def.power}");
        if (!def) return;
        switch (def.Type)
        {
            case CardType.Attack:
            {
                _enemyController.ApplyDamage(def.power);
                break;
            }
            case CardType.Defense:
                _playerController.ApplyDefense(def.power);
                break;

            case CardType.Dodge:
                _playerController.ApplyDodge(); // 다음 턴에 발동
                Debug.Log("DODGE");
                break;
        }

        _playerController.isCasting = false;
    }

    private void ProcessPlayerCasting()
    {
        _playerController.remainCastTime--;
        if (_playerController.remainCastTime <= 0)
        {
            _playerController.isCasting = false;
            ProcessPlayerCard(_playerController.castingCard);
        }
    }
    private void ProcessEnemyCard(CardDefinition cardDefinition)
    {
        CardDefinition def = cardDefinition;
        Debug.Log($"ENEMY CAST: type={def.Type}, turns={def.castTimeTurns}, value={def.power}");
        if (!def) return;
        switch (def.Type)
        {
            case CardType.Attack:
            {
                if (_playerController.isDodging) break;
                _playerController.ApplyDamage(def.power);
                break;
            }
            case CardType.Defense:
                _enemyController.ApplyDefense(def.power);
                break;

            case CardType.Dodge:
                Debug.Log("Error: Enemy tries Dodging");
                break;
        }

        _enemyController.castingCard = null;
    }

    private void ProcessEnemyCasting()
    {
        if (!_enemyController.castingCard) return;

        _enemyController.remainCastTime--;
        var card = _enemyController.castingCard;
        Debug.Log($"ENEMY CASTING: type={card.Type}, turns={card.castTimeTurns}, value={card.power}");

        if (_enemyController.remainCastTime <= 0)
        {
            ProcessEnemyCard(_enemyController.castingCard);
        }
    }
    
    private static void ApplyDamage(ref int hp, ref int defense, int damage)
    {
        damage = Math.Max(0, damage);
        if (damage == 0) return;

        int absorbed = Math.Min(defense, damage);
        defense -= absorbed;

        int remaining = damage - absorbed;
        if (remaining > 0)
            hp = Math.Max(0, hp - remaining);
    }

    private void EnemyStartCast()
    {
        if (_enemyController.castingCard) return;
       var randId= Random.Range(0, _enemyController.enemyActionList.Count-1);
       var card = _enemyController.enemyActionList[randId];
        _enemyController.StartCasting(card);
        Debug.Log($"ENEMY START CAST: type={card.Type}, turns={card.castTimeTurns}, value={card.power}");
    }
}