using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float spawnDelay = 5;
        private float spawnDelayBuffer;
        [SerializeField] private List<GameObject> enemies;
        [SerializeField] private float spawnRadius;

        private GameObject GetNewEnemy()
        {
            return enemies[Random.Range(0, enemies.Count)];
        }

        private void Update()
        {
            spawnDelayBuffer -= Time.deltaTime;
            if (spawnDelayBuffer > 0) return;
            var enemy = GetNewEnemy();
            SpawnEnemy(enemy);
            spawnDelayBuffer = spawnDelay;
        }

        private void SpawnEnemy(GameObject enemy)
        {
            Instantiate(enemy, 
                new Vector3(transform.position.x + Random.Range(0, spawnRadius), 
                transform.position.y + Random.Range(0, spawnRadius), 
                transform.position.z), 
                Quaternion.identity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
