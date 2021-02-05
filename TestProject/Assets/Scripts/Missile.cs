using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public bool IsAlive { get; set; }
    public PlayerType playerType { get; set; }

    private Vector3 targetPos;
    private bool IsWithinTargetExplodeRange {
        get {
            return Vector3.Distance(transform.position, targetPos) <= GameConfig.Instance.targetDistanceExplode;
        }
    }

    private float missileSpeed;

    public void SetTarget(Vector3 targetPos, float missileSpeed) {
        this.missileSpeed = missileSpeed;
        this.targetPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        IsAlive = true;
        StartCoroutine(MoveToTarget());
    }

    public void HandleMissile() {
        if (targetPos == null) return;

        if (IsWithinTargetExplodeRange) {
            Kill();
        }
    }

    private IEnumerator MoveToTarget() {
        float speed = missileSpeed * Time.deltaTime;
        while (true) {
            float distance = Vector3.Distance(transform.position, targetPos);

            if (IsAlive) {
                if (!GameplayManager.Instance.HasTurret) speed = (missileSpeed * 2f) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            } else {
                break;
            }
        }
    }

    public void SetMissilePosition(Vector3 missilePos) {
        this.transform.position = missilePos;
    }

    public void Restart() {
        Kill(false);
    }

    public void Kill(bool withExplosion = true) {
        if (!IsAlive) return;
        IsAlive = false;
        gameObject.SetActive(false);
        if (playerType == PlayerType.Hero) {
            GameplayManager.DeadMissiles.Enqueue(this);
        } else {
            GameplayManager.EnemyDeadMissiles.Enqueue(this);
        }
        GetComponent<TrailRenderer>().Clear();
        if (withExplosion) SpawnExplosion();
        transform.position = new Vector3(-10, -10, -10);
    }

    private void SpawnExplosion() {
        Explosion explosion = GameplayManager.Instance.GetAExplosion(transform.position, playerType);
        explosion.IsAlive = true;
        if (playerType == PlayerType.Hero) {
            explosion.SetExplosion(GameConfig.Instance.missileExplosionDurationSeconds, GameConfig.Instance.missileExplosionSize, transform.position);
            GameplayManager.ExplosionCollection.Add(explosion);
        } else {
            explosion.SetExplosion(GameConfig.Instance.enemyMissileExplosionDurationSeconds, GameConfig.Instance.enemyMissileExplosionSize, transform.position);
            GameplayManager.EnemyExplosionCollection.Add(explosion);
        }
    }
}
