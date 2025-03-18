using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class EnemyBurstSpawnArea : MonoBehaviour
{
    [SerializeField] private Collider spawnCollider;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<EnemyScriptableObject> enemies = new List<EnemyScriptableObject>();
    [SerializeField] private EnemySpawner.SpawnMethod spawnMethod = EnemySpawner.SpawnMethod.Random;
    [SerializeField] private int spawnCount = 0;
    [SerializeField] private float spawnDelay = 0.5f;

    private Coroutine spawnEnemyCoroutine;
    private Bounds Bounds;

    private void Awake()
    {
        if(spawnCollider == null)
        {
            spawnCollider = GetComponent<Collider>();
        }
        Bounds = spawnCollider.bounds;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnEnemyCoroutine == null)
        {
            spawnEnemyCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    private Vector3 GetRandomPositionBounds()
    {
        return new Vector3(Random.Range(Bounds.min.x, Bounds.max.x), Bounds.min.y, Random.Range(Bounds.min.z, Bounds.max.z));
    }
    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            if (spawnMethod == EnemySpawner.SpawnMethod.RoundRobin)
            {
                enemySpawner.DoSpawnEnemy(
                    enemySpawner.enemies.FindIndex((enemy) => enemy.Equals(enemies[i % enemies.Count])),
                    GetRandomPositionBounds()
                );

            }
            else if (spawnMethod == EnemySpawner.SpawnMethod.Random)
            {
                int index = Random.Range(0, enemies.Count);
                enemySpawner.DoSpawnEnemy(
                    enemySpawner.enemies.FindIndex((enemy) => enemy.Equals(enemies[index])),
                    GetRandomPositionBounds()
                );
            }

            yield return wait;
        }
        Destroy(gameObject);
    }

}
