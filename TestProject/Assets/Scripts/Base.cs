using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {
    public bool IsAlive {
        get {
            return gameObject.GetComponent<MeshRenderer>().enabled;
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == "EnemyMissile") {
            collider.GetComponent<Missile>().Kill(false);
            Kill();
        }
    }

    public virtual void Restart() {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public virtual void Kill() {
        GetComponent<MeshRenderer>().enabled = false;
        if (GameplayManager.Instance.IsGameOver) {
            EnemyMissileSpawner.Instance.StopSpawning();
        }
    }
}
