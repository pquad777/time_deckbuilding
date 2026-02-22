using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private AudioManager _audioManager;
    public AudioManager AudioManager => _audioManager;

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        _audioManager = GetComponent<AudioManager>();
        AudioSource[] sources = GetComponents<AudioSource>();
        _audioManager.Init(sources[0],sources[1]);
    }

    public PlayerController playerController;
    public CombatController combatController;
    [SerializeField] public List<EnemyDefinition> _enemyList = new();
    private System.Random _rng = new System.Random();
    public EnemyDefinition RandomEnemyEncounter()
    {
        if (_enemyList == null || _enemyList.Count == 0)
        {
            Debug.LogError("GameManager: _enemyList가 비어있습니다. EnemyDefinition을 추가하세요.");
            return null;
        }
        int idx = _rng.Next(0, _enemyList.Count);
        EnemyDefinition picked = _enemyList[idx];
        if (picked == null)
        {
            Debug.LogError($"_enemyList[{idx}] null");
            return null;
        }
        Debug.Log($"[Encounter] Random enemy: {picked.displayName} (index={idx})");
        return picked;
        
    }
}
