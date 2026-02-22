using System.Collections.Generic;
using UnityEngine;

public class CombatEndRouter : MonoBehaviour
{
    [SerializeField] private CombatController combat;
    [SerializeField] private GameManager gameManager;

    [Header("UI")]
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private EventUI eventUI;
    [SerializeField] private RewardUI rewardUI;
    [SerializeField] private RestUI restUI; // ✅ 추가

    [Range(0f, 1f)]
    [SerializeField] private float shopChance = 0.35f;

    [Header("Rules")]
    [SerializeField] private int combatsPerShop = 3;
    private int _winsSinceLastShop = 0;

    [Header("Encounters")]
    [SerializeField] private List<EnemyDefinition> eliteEnemies = new();
    [SerializeField] private List<EnemyDefinition> bossEnemies = new();

    private readonly List<EnemyDefinition> _eliteBag = new();
    private readonly List<EnemyDefinition> _bossBag = new();

    // cycle: 0,1,2 (2가 마지막)
    private int _cycle = 0;
    // step: 0 Normal, 1 Rest, 2 Normal, 3 Rest, 4 Elite/Boss, 5 Reward
    private int _step = 0;

    private const int LastCycleIndex = 2;

    void OnEnable()
    {
        if (combat != null) combat.OnCombatEnded += HandleCombatEnded;
    }

    void OnDisable()
    {
        if (combat != null) combat.OnCombatEnded -= HandleCombatEnded;
    }

    // ✅ StartMenu에서 “게임 시작” 버튼 누르면 이걸 호출하는 걸 추천
    public void StartRun()
    {
        _cycle = 0;
        _step = 0;
        _winsSinceLastShop = 0;
        
        _eliteBag.Clear();
        _bossBag.Clear();
        
        StartNextCombatBySchedule();
    }

    private void HandleCombatEnded(CombatController.CombatResult result)
    {
        if (result == CombatController.CombatResult.Lose)
        {
            GameFlowManager.I.SetState(GameState.GameEnd);
            return;
        }

        // 승리 카운트는 유지(기존 규칙 살리기)
        _winsSinceLastShop++;

        
        if (_step == 0 || _step == 2)
        {
            // Normal 전투 승리 → Rest
            _step++; 
            OpenRest();
            return;
        }

        if (_step == 4)
        {
            // Elite/Boss 승리 → Reward
            _step = 5;
            OpenReward();
            return;
        }

        
        OpenRest();
    }

    private void OpenRest()
    {
        if (GameFlowManager.I == null) { Debug.LogError("[Router] GameFlowManager.I is NULL"); return; }
        if (restUI == null) { Debug.LogError("[Router] restUI is NULL (assign in inspector)"); return; }

        GameFlowManager.I.SetState(GameState.Rest);
        restUI.Open(onLeave: AfterRest);
    }

    private void AfterRest()
    {
        
        if (_step == 1) _step = 2;
        else if (_step == 3) _step = 4;

        StartNextCombatBySchedule();
    }

    private void OpenReward()
    {
        GameFlowManager.I.SetState(GameState.Reward);
        rewardUI.Open(onLeave: AfterReward);
    }

    private void AfterReward()
    {
        
        if (_winsSinceLastShop >= combatsPerShop)
        {
            _winsSinceLastShop = 0;
            OpenShopOrEvent(onLeave: ContinueAfterMeta);
            return;
        }

        ContinueAfterMeta();
    }

    private void OpenShopOrEvent(System.Action onLeave)
    {
        bool goShop = Random.value < shopChance;

        if (goShop)
        {
            GameFlowManager.I.SetState(GameState.Shop);
            shopUI.Open(onLeave: onLeave);
        }
        else
        {
            GameFlowManager.I.SetState(GameState.Event);
            eventUI.OpenRandom(onLeave: onLeave);
        }
    }

    private void ContinueAfterMeta()
    {
        // Reward(5) 이후: 다음 cycle로 이동
        _cycle++;

        // 마지막(보스)까지 끝나면 종료 처리
        if (_cycle > LastCycleIndex)
        {
            GameFlowManager.I.SetState(GameState.GameEnd); // 또는 StartMenu로 보내도 됨
            return;
        }

        // 다음 사이클 시작
        _step = 0;
        StartNextCombatBySchedule();
    }

    private void StartNextCombatBySchedule()
    {
        GameFlowManager.I.SetState(GameState.Combat);
        combat.StartCombat(GetNextEnemy());
    }

    private EnemyDefinition GetNextEnemy()
    {
        // Normal fights: step 0,2
        if (_step == 0 || _step == 2)
            return gameManager.RandomEnemyEncounter(); // ✅ 그대로 유지

        // Elite/Boss fight: step 4
        if (_step == 4)
        {
            bool isLastCycle = (_cycle == LastCycleIndex);
            return isLastCycle ? DrawBossNoRepeat() : DrawEliteNoRepeat();
        }

        // 안전장치
        return gameManager.RandomEnemyEncounter();
    }

    private EnemyDefinition DrawEliteNoRepeat()
    {
        if (_eliteBag.Count == 0) RefillAndShuffle(eliteEnemies, _eliteBag);

        // 여전히 비어있으면 fallback
        if (_eliteBag.Count == 0) return gameManager.RandomEnemyEncounter();

        var e = _eliteBag[_eliteBag.Count - 1];
        _eliteBag.RemoveAt(_eliteBag.Count - 1);
        return e;
    }

    private EnemyDefinition DrawBossNoRepeat()
    {
        if (_bossBag.Count == 0) RefillAndShuffle(bossEnemies, _bossBag);

        if (_bossBag.Count == 0) return DrawEliteNoRepeat();

        var e = _bossBag[_bossBag.Count - 1];
        _bossBag.RemoveAt(_bossBag.Count - 1);
        return e;
    }
    private void RefillAndShuffle(List<EnemyDefinition> src, List<EnemyDefinition> bag)
    {
        bag.Clear();
        if (src == null || src.Count == 0) return;

        bag.AddRange(src);

        // Fisher–Yates shuffle
        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }
    }
}