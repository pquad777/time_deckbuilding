using UnityEngine;

public enum GameState { Combat, Shop, Event, Rest, GameEnd, Reward, StartMenu }

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager I { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject eventPanel;
    // [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject restPanel;
    
    [Header("Background Roots (SpriteRenderer objects)")]
    [SerializeField] private GameObject combatBgRoot; 
    [SerializeField] private GameObject shopBgRoot;   
    [SerializeField] private GameObject eventBgRoot;  
    [SerializeField] private GameObject rewardBgRoot;
    [SerializeField] private GameObject startMenuBgRoot;
    // [SerializeField] private GameObject gameEndBgRoot;
    [SerializeField] private GameObject restBgRoot;
    void Start()
    {
        SetState(GameState.StartMenu);
    }

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    public void SetState(GameState state)
    {
        
        if (combatPanel)  combatPanel.SetActive(state == GameState.Combat);
        if (shopPanel)    shopPanel.SetActive(state == GameState.Shop);
        if (eventPanel)   eventPanel.SetActive(state == GameState.Event);
        // if (gameEndPanel) gameEndPanel.SetActive(state == GameState.GameEnd);
        if (rewardPanel)  rewardPanel.SetActive(state == GameState.Reward);
        if (startMenuPanel) startMenuPanel.SetActive(state == GameState.StartMenu);
        if (restPanel) restPanel.SetActive(state == GameState.Rest);
        if (restBgRoot) restBgRoot.SetActive(state == GameState.Rest);
        if (startMenuBgRoot) startMenuBgRoot.SetActive(state == GameState.StartMenu);
        if (combatBgRoot) combatBgRoot.SetActive(state == GameState.Combat);
        if (shopBgRoot)   shopBgRoot.SetActive(state == GameState.Shop);
        if (eventBgRoot)  eventBgRoot.SetActive(state == GameState.Event);
        if (rewardBgRoot)  rewardBgRoot.SetActive(state == GameState.Reward);
        // if (gameEndBgRoot) gameEndBgRoot.SetActive(state == GameState.GameEnd);

        
        if (state == GameState.Shop)
            shopBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
        if (state == GameState.Event)
            eventBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
        if (state == GameState.Rest)
            restBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
            
    }
}