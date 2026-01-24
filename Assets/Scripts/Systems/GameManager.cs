using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public PlayerController playerController;
    public CombatController combatController;
    [SerializeField]private List<CardDefinition> _debugPlayerDeck = new();
}
