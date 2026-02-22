using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private CombatController combat;
    [SerializeField] private GameManager gameManager;

    public void OnClickStart()
    {
        GameManager.instance.AudioManager.PlaySfx(AudioType.ClickButton);
        GameFlowManager.I.SetState(GameState.Combat);
        combat.StartCombat(gameManager.RandomEnemyEncounter());
    }
    
   
    
}