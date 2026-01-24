using UnityEngine;
using System;
using System.Collections.Generic;

public class CombatController:MonoBehaviour
{
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private InputController _inputController;

    private int _handSize = 4;
    private int _defaultDiscardIndex = 0;

    [SerializeField]private List<CardDefinition> _debugPlayerDeck = new();
    private DeckSystem _deckSystem;
    private System.Random _aiRng = new System.Random();

    public class CombatState // 시뮬 용으로 간단하게 구성
    {
        public int playerHp = 300;
        public int enemyHp = 30;
        public int playercost = 10;
        public int playerDefense = 0;
        
        public bool isCasting = false;
        public int castTurnsRemaining = 0;
        public int castDamage = 0;
        
        public int enemyDefense = 0; // 영구 누적 방어(플레이어가 없애기 전까지 유지)

        public bool enemyIsCasting = false;
        public int enemyCastTurnsRemaining = 0;

        
        public EnemyCastType enemyCastType = EnemyCastType.None;
        public int enemyCastValue = 0; 
        
        public bool dodgeQueued = false;      // 다음 턴에 회피를 쓸 예정
        public bool dodgeActiveThisTurn = false; 

    }
    public enum EnemyCastType { None, Attack, Defense }
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
        
        _inputController.OnCardKeyPressed -= HandleCardKeyPressed;
        _inputController.OnCancelPressed -= HandleCancelPressed;
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
        Debug.Log($"== TURN {turnIndex} START ==");
        _combatState.dodgeActiveThisTurn = _combatState.dodgeQueued;
        _combatState.dodgeQueued = false;
        
        ProcessCastingAttack(); // 시전 턴 감소 + 완료시 데미지 적용
        ProcessEnemyCasting();
        EnemyDecideAction();
        
        bool canAct = !_combatState.isCasting;
        _inputController.Enable(canAct);
        _inputController.ClearChoice();

        
        PrintHand("Hand");
        Debug.Log($"Current Cost : {_combatState.playercost}");
        if (!canAct)
            Debug.Log($"CASTING... ({_combatState.castTurnsRemaining} turns left)");
        
    }

    private void TurnEnd(int turnIndex)
    {
        Debug.Log($"-- TURN {turnIndex} END (Resolve) --");
        _inputController.Enable(false);
        ResolveTurn();
        
        
        CheckEnd();
        
        _combatState.playerHp--;
        _combatState.dodgeActiveThisTurn = false;
        PrintHand("AfterResolve");
        PrintState();
        
    }

    private void ResolveTurn()
    {
        if (_combatState.isCasting)
        {
            _inputController.ClearChoice();
            DiscardDefault();
            _deckSystem.DrawToHand();
            _combatState.playercost = Math.Min(10, ++_combatState.playercost);
            return;
        }
        if (_inputController.ChosenIndex.HasValue) Battle();
        else
        {
            DiscardDefault();
            _combatState.playercost = Math.Min(10, ++_combatState.playercost);
        }
        _deckSystem.DrawToHand();
        _inputController.ClearChoice();
        
    }
    

    private void DiscardDefault()
    {
        int idx = 0;
        var card = _deckSystem.DiscardFromHand(idx);
        Debug.Log($"DISCARD(default): {card.Def.displayName}");
    }
    
    private void PrintHand(string label)
    {
        if (_deckSystem == null) return;

        string s = $"{label} [";
        for (int i = 3; i >= 0; i--)
        {
            s += (i == 3 ? "" : ", ") + _deckSystem.Hand[i].Def.displayName;
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
    private void CheckEnd() 
    {
        if (_combatState.enemyHp == 0)
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
        Debug.Log($"HP: {_combatState.playerHp} Def:{_combatState.playerDefense} Enemy:{_combatState.enemyHp} EnemyDef:{_combatState.enemyDefense}");
    }
    private void HandleCancelPressed()
    {
        _inputController.ClearChoice();
    }
    
    private void HandleCardKeyPressed(int idx)
    {
        if (_combatState.isCasting)
        {
            Debug.Log("Cannot act: casting in progress");
            return;
        }
        // 코스트 체크
        if (!CanUseCard(idx))
        {
            Debug.Log("Not enough cost");
            return;
        }

        _inputController.SetChoice(idx); // 선택 확정
    }

    private void Battle()
    {
        int idx = _inputController.ChosenIndex.Value;
        var card = _deckSystem.PlayFromHand(idx);
        var def = card.Def;
        _combatState.playercost -= def.cost;
        switch (def.Type)
        {
            case CardType.Attack:
            {
                int cast = Math.Max(0, def.castTimeTurns);

                if (cast == 0)
                {
                    // 즉발 공격
                    int raw = def.power;
                    ApplyDamage(ref _combatState.enemyHp, ref _combatState.enemyDefense, raw);
                    Debug.Log($"PLAYER HIT! dmg={raw} enemyDef={_combatState.enemyDefense} enemyHp={_combatState.enemyHp}");
                }
                else
                {
                    // 시전 시작: 다른 행동 불가
                    _combatState.isCasting = true;
                    _combatState.castTurnsRemaining = cast;
                    _combatState.castDamage = def.power;
                }
                break;
            }

            case CardType.Defense:
                _combatState.playerDefense += def.power;
                break;

            case CardType.Dodge:
                _combatState.dodgeQueued = true;   // 다음 턴에 발동
                Debug.Log("DODGE QUEUED (next turn)");
                break;
        }
    }
    
    private void ProcessCastingAttack()
    {
        if (!_combatState.isCasting) return;

        _combatState.castTurnsRemaining = Math.Max(0, _combatState.castTurnsRemaining - 1);

        if (_combatState.castTurnsRemaining == 0)
        {
            int raw = _combatState.castDamage;
            int beforeDef = _combatState.enemyDefense;
            int beforeHp  = _combatState.enemyHp;

            ApplyDamage(ref _combatState.enemyHp, ref _combatState.enemyDefense, raw);

            Debug.Log($"ATTACK RESOLVED! dmg={raw} enemyDef {beforeDef}->{_combatState.enemyDefense} hp {beforeHp}->{_combatState.enemyHp}");

            _combatState.isCasting = false;
            _combatState.castDamage = 0;
        }
    }
    private void ProcessEnemyCasting()
    {
        if (!_combatState.enemyIsCasting) return;

        _combatState.enemyCastTurnsRemaining = Math.Max(0, _combatState.enemyCastTurnsRemaining - 1);

        if (_combatState.enemyCastTurnsRemaining > 0) return;

        // 발동
        if (_combatState.enemyCastType == EnemyCastType.Attack)
        {
            if (_combatState.dodgeActiveThisTurn)
            {
                _combatState.dodgeActiveThisTurn = false; 
                Debug.Log("DODGED! enemy attack negated");
            }
            else
            {
                int raw = _combatState.enemyCastValue;
                int beforeDef = _combatState.playerDefense;
                int beforeHp  = _combatState.playerHp;

                ApplyDamage(ref _combatState.playerHp, ref _combatState.playerDefense, raw);

                Debug.Log($"ENEMY ATTACK! dmg={raw} playerDef {beforeDef}->{_combatState.playerDefense} hp {beforeHp}->{_combatState.playerHp}");
            }
        }
        else if (_combatState.enemyCastType == EnemyCastType.Defense)
        {
            // 적 방어는 영구 누적
            _combatState.enemyDefense += _combatState.enemyCastValue;
            Debug.Log($"ENEMY DEFENSE RESOLVED! +{_combatState.enemyCastValue} (total={_combatState.enemyDefense})");
        }

        // 캐스팅 종료
        _combatState.enemyIsCasting = false;
        _combatState.enemyCastType = EnemyCastType.None;
        _combatState.enemyCastValue = 0;
    }
    private void EnemyStartCast(EnemyCastType type, int castTurns, int value)
    {
        castTurns = Math.Max(0, castTurns);
        
        castTurns = Math.Max(1, castTurns);

        _combatState.enemyIsCasting = true;
        _combatState.enemyCastType = type;
        _combatState.enemyCastTurnsRemaining = castTurns;
        _combatState.enemyCastValue = value;

        Debug.Log($"ENEMY START CAST: type={type}, turns={castTurns}, value={value}");
    }
    private void EnemyDecideAction()
    {
        if (_combatState.enemyIsCasting)
        {
            // 캐스팅 중이면 기다리기
            return;
        }

        // 랜덤 선택: 0=Attack, 1=Defense, 2=Wait
        int choice = _aiRng.Next(3);

        if (choice == 2)
        {
            Debug.Log("ENEMY chooses WAIT");
            return;
        }

        if (choice == 0)
        {
            // 공격(수치는 임의)
            int dmg = _aiRng.Next(3, 9);         // 3~8
            int cast = _aiRng.Next(1, 4);        // 1~3턴
            EnemyStartCast(EnemyCastType.Attack, cast, dmg);
        }
        else
        {
            // 방어(수치는 임의)
            int addDef = _aiRng.Next(2, 7);      // 2~6
            int cast = _aiRng.Next(1, 3);        // 1~2턴
            EnemyStartCast(EnemyCastType.Defense, cast, addDef);
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

}
