using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    public Enemy prefab;
    public AttackScriptableObject attackConfiguration;
    // Enemy Stats
    public int health = 100;
    // Moment Stats
    public EnemyState defaultState;
    public float idleLocationRadius = 4f;
    public float idleMoveSpeedMultiplier = 0.5f;
    [Range(2, 10)]
    public int wayPoint = 4;
    public float lineOfSightRange = 6f;
    public float fieldOfView = 90f;

    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;

    public float acceleration = 8;
    public float angularSpeed = 120;
    // -1 means everything
    public int areaMask = -1;
    public int avoidancePriority = 50;
    public float baseOffset = 0;
    public float height = 2f;
    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float radius = 0.5f;
    public float speed = 3f;
    public float stoppingDistance = 0.5f;

    public EnemyScriptableObject ScaleUpForLevel(ScalingScriptableObject scaling, int level)
    {
        EnemyScriptableObject scaleUpEnemy = CreateInstance<EnemyScriptableObject>();

        scaleUpEnemy.name = name;
        scaleUpEnemy.prefab = prefab;

        scaleUpEnemy.attackConfiguration = attackConfiguration.ScaleUpForLevel(scaling, level); 

        scaleUpEnemy.health = Mathf.FloorToInt(health * scaling.HealthCurve.Evaluate(level));

        scaleUpEnemy.defaultState = defaultState;
        scaleUpEnemy.idleLocationRadius = idleLocationRadius;
        scaleUpEnemy.idleMoveSpeedMultiplier = idleMoveSpeedMultiplier;
        scaleUpEnemy.wayPoint = wayPoint;
        scaleUpEnemy.lineOfSightRange = lineOfSightRange;
        scaleUpEnemy.fieldOfView = fieldOfView;

        scaleUpEnemy.AIUpdateInterval = Mathf.Max(0.02f, AIUpdateInterval / scaling.SpeedCurve.Evaluate(level)); 
        scaleUpEnemy.acceleration = acceleration * Mathf.Clamp(scaling.SpeedCurve.Evaluate(level), 1f, 2.5f);
        scaleUpEnemy.angularSpeed = angularSpeed;

        scaleUpEnemy.areaMask = areaMask;
        scaleUpEnemy.avoidancePriority = avoidancePriority;

        scaleUpEnemy.baseOffset = baseOffset;
        scaleUpEnemy.height = height;
        scaleUpEnemy.obstacleAvoidanceType = obstacleAvoidanceType;
        scaleUpEnemy.radius = radius;
        scaleUpEnemy.speed = speed * scaling.SpeedCurve.Evaluate(level);
        scaleUpEnemy.stoppingDistance = stoppingDistance;

        return scaleUpEnemy;
    }
    public void SetUpEnemy(Enemy enemy)
    {
        enemy.agent.acceleration =  acceleration;
        enemy.agent.angularSpeed =  angularSpeed;
        enemy.agent.areaMask =  areaMask;
        enemy.agent.avoidancePriority =  avoidancePriority;
        enemy.agent.baseOffset =  baseOffset;
        enemy.agent.height =  height;
        enemy.agent.obstacleAvoidanceType = obstacleAvoidanceType;
        enemy.agent.radius =  radius;
        enemy.agent.speed =  speed;
        // agent.stoppingDistance =  stoppingDistance;

        enemy.movement.updateRate =  AIUpdateInterval;
        enemy.movement.defautState = defaultState;
        enemy.movement.idleLocationRadius = idleLocationRadius;
        enemy.movement.idleMoveSpeedMultiplier = idleMoveSpeedMultiplier;
        enemy.movement.wayPoint = new Vector3[wayPoint];
        enemy.movement.lineOfSightChecker.fieldOfView = fieldOfView;
        enemy.movement.lineOfSightChecker.sphereCollider.radius = lineOfSightRange;
        enemy.movement.lineOfSightChecker.lineOfSightLayer = attackConfiguration.lineOfSightLayers;

        enemy.health =  health;
        
        attackConfiguration.SetupEnemy(enemy);

        enemy.agent.stoppingDistance = Mathf.Max(0.5f, attackConfiguration.attackRadius * 0.8f);

        enemy.healthSlider.maxValue = health;
        enemy.healthSlider.value = health;
    }    
}
