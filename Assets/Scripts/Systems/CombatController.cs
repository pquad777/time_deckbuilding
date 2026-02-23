using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatController : MonoBehaviour
{
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private InputController _inputController;
    [SerializeField] private HandUI handUI;
    [SerializeField] private EnemySlotView enemyView;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameEndPanel gameEndPanel;
    [SerializeField] private Sprite[] countdownSprites;
    [SerializeField] private Image countdownImage;
    [SerializeField] private GameObject PlayerFilterList;
    [SerializeField] private GameObject EnemyFilterList;
    [SerializeField] private GameObject FilterPrefab;
    [SerializeField] private GameObject PlayerCastingField;
    [SerializeField] private GameObject EnemyCastingField;
    [SerializeField] private GameObject CastingPrefab; 

    private int _handSize = 4;

    private PlayerController _playerController;
    private EnemyController _enemyController = new();
    private DeckSystem _deckSystem;
    
    public enum CombatResult { Win, Lose }
    public event Action<CombatResult> OnCombatEnded;

    private bool _combatEnded = false;
    private bool _playerActedThisTurn = false;
    private bool _playerAutoDiscardedThisTurn = false;
    
    // ===== Timeline & Filters =====
    private const int TimelineHorizon = 12;
    private List<ActionEvent>[] _timeline;
    private readonly List<IEventFilter> _filters = new();
    private int _currentTurn;
    
    public void StartCombat(EnemyDefinition enemyDefinition)
    {
        ClearAllChildren();
        _combatEnded = false;

        _enemyController.LoadInfo(enemyDefinition);
        enemyView.Bind(_enemyController);
        GameManager.instance.AudioManager.ChangeVolume(0.3f);
        _playerController = GameManager.instance.playerController;

        _deckSystem = new DeckSystem(_handSize);
        var rng = new System.Random();
        _deckSystem.Init(_playerController.playerDeck, rng);
        _deckSystem.InitHand(_handSize);
        handUI.Init(_deckSystem);

        InitTimeline();

        countdownImage.enabled = true;
        _countdown = 3;
        countdownImage.sprite = countdownSprites[_countdown];
        gameManager.AudioManager.PlaySfx(AudioType.TurnEnd1);
        _playerController.EndCastLock();
        _turnManager.OnTurnEnd += Countdown;
            enemyView.Appear();
        _turnManager.StartLoop(true,false);
        
    }
    private int _countdown=3;
    private void Countdown(int _)
    {
        _countdown--;
        countdownImage.sprite = countdownSprites[_countdown];
        if (_countdown == 0)
        {
            countdownImage.enabled = false;
            _turnManager.OnTurnEnd -= Countdown;
            _turnManager.OnTurnStart += TurnStart;
            _turnManager.OnTurnEnd += TurnEnd;

            _inputController.OnCardKeyPressed += HandleCardKeyPressed;
            _inputController.OnCancelPressed += HandleCancelPressed;

            gameManager.AudioManager.PlaySfx(AudioType.BattleStart);
            GameManager.instance.AudioManager.ChangeVolume(1f);
            enemyView.FinishAppearing();
            _turnManager.StartLoop(true);
            PrintHand("Combat Start Hand");
        }
        else
        {
            gameManager.AudioManager.PlaySfx(AudioType.TurnEnd1);
        }
    }
    private void InitTimeline()
    {
        _timeline = new List<ActionEvent>[TimelineHorizon];
        for (int i = 0; i < TimelineHorizon; i++)
            _timeline[i] = new List<ActionEvent>(8);
    }
    
    public void AddFilter(IEventFilter f)
    {
        if (f == null) return;
        if (!_filters.Contains(f)) _filters.Add(f);
    }

    public void RemoveFilter(IEventFilter f)
    {
        if (f == null) return;
        _filters.Remove(f);
    }

    public void Schedule(int delay, ActionEvent e)
    {
        delay = Mathf.Clamp(delay, 0, TimelineHorizon - 1);
        _timeline[delay].Add(e);
    }

    public void CancelScheduledById(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        for (int i = 0; i < TimelineHorizon; i++)
        {
            var list = _timeline[i];
            for (int j = list.Count - 1; j >= 0; j--)
            {
                if (list[j].id == id) list.RemoveAt(j);
            }
        }
    }

    private void AddCastingInfo(GameObject where, ActionEvent e)
    {
        GameObject Info = Instantiate(CastingPrefab,where.transform);
        Image image = Info.GetComponentInChildren<Image>();
        TextMeshProUGUI text =  Info.GetComponentInChildren<TextMeshProUGUI>();
        switch (e.type)
        {
            case ActionType.Damage:
            {
                image.sprite = Resources.Load<Sprite>("Sprites/AttackIcon");
                text.text = e.value.ToString();
                break;
            }
            case ActionType.GainBlock:
            {
                image.sprite = Resources.Load<Sprite>("Sprites/DefenceIcon");
                text.text = e.value.ToString();
                break;
            }
            case ActionType.Dodge:
            {
                image.sprite = Resources.Load<Sprite>("Sprites/Evade");
                break;
            }
            case ActionType.Filter:
            {
                image.sprite = Resources.Load<Sprite>("Sprites/Effect");
                break;
            }
        }
    }
    private void AddFilterInfo(GameObject where,IEventFilter filter)
    {
        GameObject Info = Instantiate(FilterPrefab,where.transform);
        Image image = Info.GetComponentInChildren<Image>();
        TextMeshProUGUI text =  Info.GetComponentInChildren<TextMeshProUGUI>();
        image.sprite = filter.ReturnSprite();
        text.text = filter.Remains.ToString();
    }

    private void ClearAllChildren()
    {
        ClearChildren(PlayerCastingField.transform);
        ClearChildren(EnemyCastingField.transform);
        ClearChildren(EnemyFilterList.transform);
        ClearChildren(PlayerFilterList.transform);
    }
    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    private void TurnStart(int turnIndex)
    {
        ClearAllChildren();
        int ClosestPlayerAction = -1;
        int ClosestEnemyAction = -1;
        for (int i = 0; i < TimelineHorizon-1; i++)
        {
            foreach(ActionEvent _event in _timeline[i])
            {
                if (_event.source == Team.Player)
                {
                    if (ClosestPlayerAction == -1) 
                    {
                        GameObject Info = Instantiate(CastingPrefab,PlayerCastingField.transform);
                        Image image = Info.GetComponentInChildren<Image>();
                        TextMeshProUGUI text =  Info.GetComponentInChildren<TextMeshProUGUI>();
                        image.sprite = Resources.Load<Sprite>("Sprites/CastingTurn");
                        text.text = (i+1).ToString();
                        ClosestPlayerAction = i;
                    }

                    if (i == ClosestPlayerAction)
                    {
                        AddCastingInfo(PlayerCastingField,_event);
                    }
                }
                if (_event.source == Team.Enemy)
                {
                    if (ClosestEnemyAction == -1) 
                    {
                        GameObject Info = Instantiate(CastingPrefab,EnemyCastingField.transform);
                        Image image = Info.GetComponentInChildren<Image>();
                        TextMeshProUGUI text =  Info.GetComponentInChildren<TextMeshProUGUI>();
                        image.sprite = Resources.Load<Sprite>("Sprites/CastingTurn");
                        text.text = (i+1).ToString();
                        ClosestEnemyAction = i;
                    }

                    if (i == ClosestEnemyAction)
                    {
                        AddCastingInfo(EnemyCastingField,_event);
                    }
                }
            }
        }
        foreach(IEventFilter filter in _filters)
        {
            if (filter.Owner == Team.Player)
            {
                AddFilterInfo(PlayerFilterList, filter);
            }
            else if (filter.Owner == Team.Enemy)
            {
                AddFilterInfo(EnemyFilterList, filter);
            }
        }
        _playerActedThisTurn = false;
        _playerAutoDiscardedThisTurn = false;
        _currentTurn = turnIndex;
        Debug.Log($"== TURN {turnIndex} START ==");
        _playerController.TickActionBlock();
        
        _playerController.EndDodge();

        EnemyPlanIntent();

        if (_deckSystem.HandCount == 4)
        {
            DiscardDefault();
            _playerController.GainCost(1);
        }
        _deckSystem.DrawToHand();

        bool canAct = !_playerController.IsLocked(_currentTurn) && !_playerController.IsActionBlocked;
        _inputController.Enable(canAct);
        for (int i = 0; i < _deckSystem.HandCount; i++)
        {
            if (!_deckSystem.HasCard(i)) continue;
            if (!canAct || _playerController.cost < _deckSystem.Hand[i].def.cost)
            {
                handUI.DisplayDisableSlot(i);
            }
            else
            {
                handUI.DisplayEnableSlot(i);
            }
        }
        HandleCancelPressed();
    }

    private void TurnEnd(int turnIndex)
    {
        Debug.Log($"-- TURN {turnIndex} END (Resolve) --");
        gameManager.AudioManager.PlaySfx(AudioType.TurnEnd1);
        _inputController.Enable(false);

        // 1) 플레이어 선택 카드 예약(이번 턴에 플레이한 것이 castTime에 따라 미래에 실행)
        PlanPlayerAction();

        // 2) 이번 턴 실행 + shift
        ResolveTimeline();

        // 3) 종료 체크
        CheckEnd();
        
        _playerController.ApplyDamage(1,false);

        PrintHand("AfterResolve");
        PrintState();
    }

    private void PlanPlayerAction()
    {
        if (!_inputController.ChosenIndex.HasValue) return;

        int idx = _inputController.ChosenIndex.Value;

        if (!CanUseCard(idx))
        {
            HandleCancelPressed();
            return;
        }

        var card = _deckSystem.PlayFromHand(idx);
        var def = card.def;
        gameManager.AudioManager.PlaySfx(AudioType.UseCard);
        // 비용 지불
        _playerController.SpendCost(def.cost);
        _playerController.StartCastLock(_currentTurn, def.castTimeTurns);
        // 카드 효과 예약(현재는 Attack/Defense/Dodge만)
        ScheduleCardEffects(def, Team.Player);
        _playerActedThisTurn = true;
        HandleCancelPressed();
    }

    private void ResolveTimeline()
    {
        var events = _timeline[0];

        ExecuteEvents(events);
        events.Clear();

        ShiftTimeline();

    }

    private void ShiftTimeline()
    {
        for (int i = 0; i < TimelineHorizon - 1; i++)
        {
            var tmp = _timeline[i];
            _timeline[i] = _timeline[i + 1];
            _timeline[i + 1] = tmp;
        }
        _timeline[TimelineHorizon - 1].Clear();
    }

    private void ExecuteEvents(List<ActionEvent> events)
    {
        if (events == null || events.Count == 0) return;

        _filters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        var ctx = new ResolveContext(this);

        for (int n = 0; n < events.Count; n++)
        {
            var e = events[n];

            // 반복(연타)
            int reps = Mathf.Max(1, e.repeat);
            for (int r = 0; r < reps; r++)
            {
                var cur = e;

                // before filters
                for (int i = 0; i < _filters.Count; i++)
                {
                    _filters[i].BeforeExecute(ref cur, ctx);
                    if (cur.cancelled) break;
                }
                if (cur.cancelled) continue;

                ExecuteOne(cur);

                // after filters
                for (int i = 0; i < _filters.Count; i++)
                    _filters[i].AfterExecute(cur, ctx);
            }
        }
    }

    private void ExecuteOne(ActionEvent e)
    {
        if (e.type == ActionType.Filter)
        {
            AddFilter(e.filter);
            Debug.LogWarning($"{e.value} {e.filter.Priority}");
            return;
        }
        bool sourceIsPlayer = e.source == Team.Player;

        // 타겟 해석
        bool targetIsOpponent = e.target == TargetPolicy.Opponent;

        if (targetIsOpponent)
        {
            // Player -> Enemy
            if (sourceIsPlayer)
            {
                if (e.type == ActionType.Damage)
                {
                    GameManager.instance.AudioManager.PlaySfx(AudioType.TryAttack);
                    _enemyController.ApplyDamage(e.value);
                }
                return;
            }
            // Enemy -> Player
            else
            {
                if (e.type == ActionType.Damage)
                {
                    GameManager.instance.AudioManager.PlaySfx(AudioType.TryAttack);
                    _playerController.ApplyDamage(e.value);
                }
                return;
            }
        }

        // Self
        if (sourceIsPlayer)
        {
            switch (e.type)
            {
                case ActionType.GainBlock: _playerController.ApplyDefense(e.value); break;
                case ActionType.Dodge: _playerController.ApplyDodge(); break;
                case ActionType.ClearBlock: _playerController.SetDefense(0); break;
            }
        }
        else
        {
            switch (e.type)
            {
                case ActionType.GainBlock: _enemyController.ApplyDefense(e.value); break;
                case ActionType.ClearBlock: _enemyController.defense = 0; break;
            }
        }
    }

    private int enemyCastingTurnRemaining = 0;
    private void EnemyPlanIntent()
    {
        if (enemyCastingTurnRemaining > 0)
        {
            enemyCastingTurnRemaining--;
            return;
        }
        var list = _enemyController.enemyActionList;
        if (list == null || list.Count == 0) return;

        int randId = Random.Range(0, list.Count);
        var card = list[randId];
        if (card == null)
        {
            Debug.LogError($"[EnemyPlanIntent] enemyActionList[{randId}] is NULL. Fix enemyActionList references.");
            return;
        }

        ScheduleCardEffects(card, Team.Enemy);

        Debug.Log($"ENEMY PLAN: {card.displayName} in {card.castTimeTurns} turns");
        enemyCastingTurnRemaining = card.castTimeTurns-1;
    }

    private void ScheduleCardEffects(CardDefinition def, Team sourceTeam)
    {
        if (def == null)
        {
            Debug.LogError($"[ScheduleCardEffects] NULL CardDefinition (team={sourceTeam})");
            return;
        }

        if (def.effects == null)
        {
            Debug.LogError($"[ScheduleCardEffects] '{def.displayName}' effects is NULL (team={sourceTeam})");
            return;
        }

        if (def.effects.Count == 0)
        {
            Debug.LogError($"[ScheduleCardEffects] '{def.displayName}' has 0 effects (team={sourceTeam})");
            return;
        }
        
        int delay = Mathf.Max(0, def.castTimeTurns);

        foreach (var effect in def.effects)
        {
            if (effect == null)
            {
                Debug.LogError($"[ScheduleCardEffects] '{def.displayName}' has NULL effect entry");
                continue;
            }

            switch (effect.type)
            {
                case EffectType.Damage:
                    Schedule(delay, ActionEvent.Damage(
                        sourceTeam,
                        TargetPolicy.Opponent,
                        effect.value,
                        true));
                    break;

                case EffectType.GainBlock:
                    Schedule(delay, ActionEvent.GainBlock(
                        sourceTeam,
                        TargetPolicy.Self,
                        effect.value));
                    break;

                case EffectType.Dodge:
                    Schedule(delay, ActionEvent.Dodge(sourceTeam));
                    break;

                case EffectType.ApplyFilter:
                    Debug.LogWarning("filterScedule");
                    Team filterOwner;
                    if (sourceTeam == Team.Player)
                    {
                        filterOwner = effect.filterTarget == TargetPolicy.Opponent ? Team.Enemy : Team.Player;
                    }
                    else
                    {
                        filterOwner = effect.filterTarget == TargetPolicy.Opponent ? Team.Player: Team.Enemy;
                    }
                    var filter = FilterFactory.Create(effect.filterType, filterOwner, effect.stacks, effect.magnitude);
                    if (filter != null)
                    {
                        Schedule(delay, ActionEvent.ApplyFilter(
                            sourceTeam,
                            effect.filterTarget,
                            filter));
                    }
                    break;
                
                case EffectType.DotDamage:
                {
                    int duration = Mathf.Max(1, effect.stacks);
                    int dpt = Mathf.Max(0, effect.value);      
                    
                    for (int t = 1; t <= duration; t++)
                    {
                        Schedule(delay + t, ActionEvent.Damage(
                            sourceTeam,
                            TargetPolicy.Opponent,
                            dpt,
                            isAttack: false   
                        ));
                    }
                    break;
                }
                
                case EffectType.AllCostDamage:
                {
                    int spent = _playerController.cost;     
                    if (spent <= 0) break;
                    
                    _playerController.SpendCost(spent);

                    int mult = Mathf.Max(0, effect.magnitude);
                    int dmg = spent * mult;

                    Schedule(delay, ActionEvent.Damage(
                        sourceTeam,
                        TargetPolicy.Opponent,
                        dmg,
                        true
                    ));
                    break;
                }
            }
        }
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
        for (int i = _deckSystem.HandCount - 1; i >= 0; i--)
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
        if (_combatEnded) return;

        if (_enemyController.health <= 0) EndCombat(CombatResult.Win);
        else if (_playerController.health <= 0) EndCombat(CombatResult.Lose);
    }

    private void EndCombat(CombatResult result)
    {
        _combatEnded = true;

        Debug.Log(result == CombatResult.Win ? "WIN" : "LOSE");

        _turnManager.OnTurnStart -= TurnStart;
        _turnManager.OnTurnEnd -= TurnEnd;

        _inputController.OnCardKeyPressed -= HandleCardKeyPressed;
        _inputController.OnCancelPressed -= HandleCancelPressed;

        _turnManager.EndLoop();
        OnCombatEnded?.Invoke(result);
    }

    private void PrintState()
    {
        Debug.Log($"HP: {_playerController.health} Def:{_playerController.defense} Enemy:{_enemyController.health} EnemyDef:{_enemyController.defense}");
    }

    private void HandleCancelPressed()
    {
        GameManager.instance.AudioManager.PlaySfx(AudioType.SelectCard);
        _inputController.ClearChoice();
        handUI.UnhighlightSlots();
    }

    private void HandleCardKeyPressed(int idx)
    {
        if (!CanUseCard(idx))
        {
            GameManager.instance.AudioManager.PlaySfx(AudioType.SelectCardFail);
            return;
        }
        handUI.HighlightSlot(idx);
        GameManager.instance.AudioManager.PlaySfx(AudioType.SelectCard);
        _inputController.SetChoice(idx);
    }
}
