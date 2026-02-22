using UnityEngine;

public class CombatEndRouter : MonoBehaviour
{
    [SerializeField] private CombatController combat;
    [SerializeField] private GameManager gameManager;
    
    [Header("UI")] 
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private EventUI eventUI;
    [SerializeField] private RewardUI rewardUI;
    // [SerializeField] private GameEndPanel gameEndPanel;

    [Range(0f, 1f)] 
    [SerializeField] private float shopChance = 0.35f;
    [Header("Rules")]
    [SerializeField] private int combatsPerShop = 3;
    private int _winsSinceLastShop = 0;
    void OnEnable()
    {
        if (combat != null) combat.OnCombatEnded += HandleCombatEnded;
    }

    void OnDisable()
    {
        if (combat != null) combat.OnCombatEnded -= HandleCombatEnded;
    }

    private void HandleCombatEnded(CombatController.CombatResult result)
    {
        if (result == CombatController.CombatResult.Lose)
        {
            GameFlowManager.I.SetState(GameState.GameEnd);

            return;
        }
        _winsSinceLastShop++;
        OpenReward();
    }

    private void StartNextCombat()
    {
        GameFlowManager.I.SetState(GameState.Combat);
        combat.StartCombat(gameManager.RandomEnemyEncounter());
    }
    private void AfterReward()
    {
        // ✅ 3승마다 상점 강제
        if (_winsSinceLastShop >= combatsPerShop)
        {
            _winsSinceLastShop = 0;
            OpenShopOrEvent();
        }
        else
        {
            StartNextCombat();
        }
    }
    
    private void OpenReward()
    {
        GameFlowManager.I.SetState(GameState.Reward);
        rewardUI.Open(onLeave: AfterReward);
    }

    private void OpenShopOrEvent()
    {
        bool goShop = Random.value < shopChance;
        if (goShop)
        {
            GameFlowManager.I.SetState(GameState.Shop);
            shopUI.Open(onLeave: StartNextCombat);
        }
        else
        {
            GameFlowManager.I.SetState(GameState.Event);
            eventUI.OpenRandom(onLeave: StartNextCombat);
        }

    }
}