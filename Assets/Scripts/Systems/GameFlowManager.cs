using UnityEngine;

public enum GameState { Combat, Shop, Event, Rest, GameEnd, Reward, StartMenu, GameLose }

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager I { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject restPanel;
    [SerializeField] private GameObject gameLosePanel;
    
    [Header("Background Roots (SpriteRenderer objects)")]
    [SerializeField] private GameObject combatBgRoot; 
    [SerializeField] private GameObject shopBgRoot;   
    [SerializeField] private GameObject eventBgRoot;  
    [SerializeField] private GameObject rewardBgRoot;
    [SerializeField] private GameObject startMenuBgRoot;
    [SerializeField] private GameObject gameEndBgRoot;
    [SerializeField] private GameObject restBgRoot;
    [SerializeField] private GameObject gameLoseBgRoot;
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
        switch(state)
        {
            case GameState.Combat:
                GameManager.instance.AudioManager.PlayBgm(AudioType.BattleBGM);
                break;
            case GameState.Shop:
                GameManager.instance.AudioManager.PlayBgm(AudioType.ShopBGM);
                break;
            case GameState.StartMenu:
                GameManager.instance.AudioManager.PlayBgm(AudioType.TitleBGM);
                break;
            case GameState.Reward:
                GameManager.instance.AudioManager.StopBgm();
                break;
            case GameState.GameEnd:
                GameManager.instance.AudioManager.PlayBgm(AudioType.GameWinBGM); 
                break;
            case GameState.GameLose: 
                GameManager.instance.AudioManager.PlayBgm(AudioType.GameLoseBGM);
                break;
        }
        if (combatPanel) combatPanel.SetActive(state == GameState.Combat);
        if (shopPanel) shopPanel.SetActive(state == GameState.Shop);
        if (eventPanel)   eventPanel.SetActive(state == GameState.Event);
        if (gameEndPanel) gameEndPanel.SetActive(state == GameState.GameEnd);
        if (rewardPanel) rewardPanel.SetActive(state == GameState.Reward);
        if (startMenuPanel) startMenuPanel.SetActive(state == GameState.StartMenu);
        if (gameLosePanel) gameLosePanel.SetActive(state == GameState.GameLose);
        
        
        if (restPanel) restPanel.SetActive(state == GameState.Rest);
        if (restBgRoot) restBgRoot.SetActive(state == GameState.Rest);
        if (startMenuBgRoot) startMenuBgRoot.SetActive(state == GameState.StartMenu);
        if (combatBgRoot) combatBgRoot.SetActive(state == GameState.Combat);
        if (shopBgRoot)   shopBgRoot.SetActive(state == GameState.Shop);
        if (eventBgRoot)  eventBgRoot.SetActive(state == GameState.Event);
        if (rewardBgRoot)  rewardBgRoot.SetActive(state == GameState.Reward);
        if (gameEndBgRoot) gameEndBgRoot.SetActive(state == GameState.GameEnd);
        if (gameLoseBgRoot) gameLoseBgRoot.SetActive(state == GameState.GameLose);

        
        if (state == GameState.Shop)
            shopBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
        if (state == GameState.Event)
            eventBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
        if (state == GameState.Rest)
            restBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
        if (state == GameState.GameEnd)
            gameEndBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
        if (state == GameState.GameLose)
            gameLoseBgRoot?.GetComponentInChildren<SingleBackgroundFitter>()?.Apply();
            
    }
}