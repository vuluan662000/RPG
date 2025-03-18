using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public int numberOfEnemiesToSpawn = 5;
    public float spawnDelay = 1f;
    public List<EnemyScriptableObject> enemies = new List<EnemyScriptableObject>();
    public SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;
    public bool continuousSpawning;
    public ScalingScriptableObject scaling;
    [Space]
    [Header("Read At runtime")]
    [SerializeField] private int level = 0;
    [SerializeField] 
    private List<EnemyScriptableObject> scaledEnemies = new List<EnemyScriptableObject>();

    private int enemiesAlive = 0;
    private int spawnedEnemies = 0;
    private int initialEnemiesToSpawn;
    private float initialSpawnDelay;

    private NavMeshTriangulation triangulation;
    private Dictionary<int, ObjectPool> enemyObjectPools = new Dictionary<int, ObjectPool>();

    private void Awake()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyObjectPools.Add(i, ObjectPool.CreateInstance(enemies[i].prefab, numberOfEnemiesToSpawn));
        }

        initialEnemiesToSpawn = numberOfEnemiesToSpawn;
        initialSpawnDelay = spawnDelay;
    }

    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation();
        for (int i = 0; i < enemies.Count; ++i) 
        {
            scaledEnemies.Add(enemies[i].ScaleUpForLevel(scaling, 0));
        }
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        level++;
        spawnedEnemies = 0;
        enemiesAlive = 0;

        for (int i = 0; i < enemies.Count ; i++)
        {
            scaledEnemies[i] = enemies[i].ScaleUpForLevel(scaling, level);
        }
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);
        
        while (spawnedEnemies < numberOfEnemiesToSpawn)
        {
            if(enemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if (enemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }
            spawnedEnemies++;

            yield return wait;
        }

        if(continuousSpawning)
        {
            ScaleUpSpawns();
            StartCoroutine(SpawnEnemies());
        }    
    }  
    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int SpawnIndex = spawnedEnemies % enemies.Count;

        DoSpawnEnemy(SpawnIndex, ChoseRandomPositionOnNavMesh());
    }
    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, enemies.Count), ChoseRandomPositionOnNavMesh());
    }
    private Vector3 ChoseRandomPositionOnNavMesh()
    {
        int vertexIndex = Random.Range(0, triangulation.vertices.Length);
        return triangulation.vertices[vertexIndex];
    }
    public void DoSpawnEnemy(int spawnIndex, Vector3 spawnPosition)
    {
        PoolableObject poolableObject = enemyObjectPools[spawnIndex].GetObject();

        if (poolableObject != null) 
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();
            scaledEnemies[spawnIndex].SetUpEnemy(enemy);

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(spawnPosition, out Hit, 2f, -1))
            {
                enemy.agent.Warp(Hit.position);
                enemy.movement.player = player;
                enemy.movement.Triangulation = triangulation;
                enemy.agent.enabled = true;
                enemy.movement.Spawn();
                enemy.OnDie = null;
                enemy.OnDie += HandleEnemyDeath;
                enemiesAlive++;
            }
            else
            {
                Debug.LogError($"unable to place NavMeshAgent on NavMesh. Tried to Use {spawnPosition}");
            }
        }
    }

    private void ScaleUpSpawns()
    {
        numberOfEnemiesToSpawn = Mathf.FloorToInt(numberOfEnemiesToSpawn * scaling.SpawnCountCurve.Evaluate(level + 1));
        spawnDelay *= initialSpawnDelay * scaling.SpawnRateCurve.Evaluate(level + 1);
    }
    private void HandleEnemyDeath(Enemy enemy)
    {
        enemiesAlive--;
        if (enemiesAlive == 0 && spawnedEnemies == numberOfEnemiesToSpawn) 
        {
            ScaleUpSpawns();
            StartCoroutine (SpawnEnemies());
        }
    }
    public enum SpawnMethod
    {
        RoundRobin,
        Random
    }
}
