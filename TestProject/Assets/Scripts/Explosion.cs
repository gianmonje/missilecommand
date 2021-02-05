using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public bool IsAlive { get; set; }
    public PlayerType playerType { get; set; }

    private Size targetSize;

    private void OnTriggerEnter(Collider collider) {
        if (!IsAlive) return;
        if (!collider.GetComponent<Missile>().IsAlive) return;
        if (collider.tag == "EnemyMissile" && playerType == PlayerType.Hero) {
            collider.GetComponent<Missile>().Kill();
        }
    }

    public void SetExplosion(float explosionDuration, float explosionSize, Vector3 explosionPosition) {
        transform.localScale = new Vector3(0, 0, 0);
        transform.position = explosionPosition;

        StartCoroutine(ScaleOverTime((explosionDuration / 2), explosionSize, delegate {
            StartCoroutine(ScaleOverTime((explosionDuration / 2), 0, delegate {
                Kill();
            }));
        }));
    }

    public void Restart() {
        Kill();
    }

    private IEnumerator ScaleOverTime(float time, float explosionSize, Action actionDone = null) {
        Vector3 originalScale = transform.localScale;
        Vector3 destinationScale = new Vector3(explosionSize, explosionSize, explosionSize);

        float currentTime = 0.0f;

        do {
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
        if (actionDone != null) actionDone();
    }

    public void Kill() {
        if (!IsAlive) return;
        IsAlive = false;
        gameObject.SetActive(false);
        if (playerType == PlayerType.Hero) {
            GameplayManager.DeadExplosion.Enqueue(this);
        } else {
            GameplayManager.EnemyDeadExplosion.Enqueue(this);
        }
        transform.position = new Vector3(-10, -10, -10);
    }
}
