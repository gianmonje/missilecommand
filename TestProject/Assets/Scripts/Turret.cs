using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Base {

    private bool isOkToFire = true;

    public bool IsOkToFire {
        get {
            return isOkToFire;
        }
    }

    public void FireMissile(Vector3 targetPos) {
        if (IsOkToFire) StartCoroutine(Fire(targetPos));
    }

    private IEnumerator Fire(Vector3 targetPos) {
        isOkToFire = false;
        Missile missile = GameplayManager.Instance.GetAMissile(transform.position, PlayerType.Hero);
        missile.SetMissilePosition(transform.position);
        missile.SetTarget(targetPos, GameConfig.Instance.missileSpeed);
        GameplayManager.MissileCollection.Add(missile);
        yield return new WaitForSeconds(GameConfig.Instance.turretRateOfFireSeconds);
        isOkToFire = true;
    }
}
