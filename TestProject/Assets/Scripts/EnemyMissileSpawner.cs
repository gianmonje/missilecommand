using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileSpawner : Singleton<EnemyMissileSpawner> {

    public void StartSpawning() {
        StartCoroutine(SpawnObject(GameConfig.Instance.delayAndSpawnRate));
    }

    public void StopSpawning() {
        StopAllCoroutines();
    }

    IEnumerator SpawnObject(float firstDelay) {
        float spawnRateCountdown = GameConfig.Instance.timeUntilSpawnRateIncrease;
        float spawnCountdown = firstDelay;
        while (true) {
            yield return null;
            spawnRateCountdown -= Time.deltaTime;
            spawnCountdown -= Time.deltaTime;

            // Should a new object be spawned?
            if (spawnCountdown < 0) {
                spawnCountdown += GameConfig.Instance.delayAndSpawnRate;
                Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Screen.height + 5, 0));
                spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
                Missile enemyMissile = GameplayManager.Instance.GetAMissile(spawnPosition, PlayerType.Enemy);
                enemyMissile.SetTarget(GameplayManager.Instance.GetRandomTarget().position, GameConfig.Instance.enemyMissileSpeed);
            }

            // Should the spawn rate increase?
            if (spawnRateCountdown < 0 && GameConfig.Instance.delayAndSpawnRate > 1) {
                spawnRateCountdown += GameConfig.Instance.timeUntilSpawnRateIncrease;
                GameConfig.Instance.delayAndSpawnRate -= 0.1f;
            }
        }
    }
}