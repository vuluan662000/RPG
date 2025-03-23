using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerController playerController;
    public float _speed = 7f;
    public int health;
    public int maxHealth = 100;
    public float _attackRange = 1.5f;
    public int _attackDamage = 20;
    private float statIncreasePerLevel = 0.2f;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        GameEvents.OnEnemyLevelUp += UpdateStats;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyLevelUp -= UpdateStats;
    }

    private void UpdateStats(int level)
    {
        float statMultiplier = 1 + (level * statIncreasePerLevel);

        _attackDamage = Mathf.FloorToInt(_attackDamage * statMultiplier);
        maxHealth = Mathf.FloorToInt(maxHealth * statMultiplier);

        health = maxHealth;
        playerController.healthSlider.maxValue = health;
        playerController.healthSlider.value = health;
    }
}
