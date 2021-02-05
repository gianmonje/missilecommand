using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Custom/Game Config")]
public class GameConfig : ScriptableObject {
    private static GameConfig instance;
    public static GameConfig Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<GameConfig>("Configs/GameConfig");
            }
            return instance;
        }
    }

    public float missileSpeed = 3;
    public float missileExplosionSize = 2;
    public float missileExplosionDurationSeconds = 2;
    public float turretRateOfFireSeconds = 1;
    public float targetDistanceExplode = 0.5f;

    public float enemyMissileSpeed = 3;
    public float enemyMissileExplosionSize = 2;
    public float enemyMissileExplosionDurationSeconds = 2;

    public float delayAndSpawnRate = 2;
    public float timeUntilSpawnRateIncrease = 30;

    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject playerMissileExplosionPrefab;
    public GameObject enemyMissileExplosionPrefab;
}
