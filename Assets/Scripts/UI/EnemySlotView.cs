using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemySlotView : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text EnemyHp;
    [SerializeField] private TMP_Text EnemyDef;

    private EnemyController _enemy;

    public void Bind(EnemyController enemy)
    {
        _enemy = enemy;
        _enemy.OnChanged += Refresh;
        nameText.text = enemy.displayName;
        portrait.sprite = enemy.sprite;
        EnemyHp.text = enemy.health.ToString();
        EnemyDef.text = enemy.defense.ToString();
        

        Refresh();
    }

    public void Refresh()
    {
        if (_enemy == null) return;

        EnemyHp.text = $"{_enemy.health}/{_enemy.maxHealth}";
        EnemyDef.text = _enemy.defense > 0 ? _enemy.defense.ToString() : "";
    }
}
