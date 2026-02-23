using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemySlotView : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text EnemyHp;
    [SerializeField] private TMP_Text EnemyDef;
    [SerializeField] private GameObject EnemyDisplay;
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private GameObject Def;
    private float HpBarOriginSize;

    private EnemyController _enemy;
    private Vector3 _origin;

    public void Bind(EnemyController enemy)
    {
        _enemy = enemy;
        HpBarOriginSize = 380f;
        _enemy.OnChanged += Refresh;
        nameText.text = enemy.displayName;
        portrait.sprite = enemy.sprite;
        EnemyHp.text = enemy.health.ToString();
        EnemyDef.text = enemy.defense.ToString();
        Def.SetActive(enemy.defense > 0);
        

        Refresh();
    }

    public void Refresh()
    {
        if (_enemy == null) return;

        EnemyHp.text = $"{_enemy.health}/{_enemy.maxHealth}";
        EnemyDef.text = _enemy.defense > 0 ? _enemy.defense.ToString() : "";
        hpBar.sizeDelta = new Vector2(HpBarOriginSize * _enemy.health/_enemy.maxHealth, hpBar.sizeDelta.y);
        hpBar.anchoredPosition = new Vector2(-(HpBarOriginSize-hpBar.sizeDelta.x)/2, hpBar.anchoredPosition.y);
        Def.SetActive(_enemy.defense > 0);
    }

    public void Appear()
    {
        _origin = EnemyDisplay.transform.position;
        EnemyDisplay.transform.position = _origin+new Vector3(600,0,0);
    }

    public void FinishAppearing()
    {
        EnemyDisplay.transform.position = _origin;
    }
    void Update()
    {
        EnemyDisplay.transform.position = Vector3.Lerp(EnemyDisplay.transform.position, _origin, Time.deltaTime);
    }
}
