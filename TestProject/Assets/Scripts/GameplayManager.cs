using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager> {

    public GameObject gameOverUI;

    public Transform turretMissileParent;
    public Transform enemyMissileParent;
    public Transform heroExplosionParent;
    public Transform enemyExplosionParent;

    public List<Turret> turretCollection;
    public List<Base> baseCollection;

    static public List<Missile> MissileCollection;
    static public Queue<Missile> DeadMissiles;

    static public List<Missile> EnemyMissileCollection;
    static public Queue<Missile> EnemyDeadMissiles;

    static public List<Explosion> ExplosionCollection;
    static public Queue<Explosion> DeadExplosion;

    static public List<Explosion> EnemyExplosionCollection;
    static public Queue<Explosion> EnemyDeadExplosion;

    private List<Transform> targetCollection;
    private List<Transform> TargetCollection {
        get {
            if (targetCollection == null) {
                targetCollection = new List<Transform>();
                for (int i = 0; i < turretCollection.Count; i++) {
                    targetCollection.Add(turretCollection[i].transform);
                }
                for (int i = 0; i < baseCollection.Count; i++) {
                    targetCollection.Add(baseCollection[i].transform);
                }
            }
            return targetCollection;
        }
    }

    public bool IsGameOver {
        get {
            for (int i = 0; i < baseCollection.Count; i++) {
                if (baseCollection[i].IsAlive) {
                    return false;
                }
            }
            gameOverUI.SetActive(true);
            KillAll();
            return true;
        }
    }

    public bool HasTurret {
        get {
            for (int i = 0; i < turretCollection.Count; i++) {
                if (turretCollection[i].IsAlive) {
                    return true;
                }
            }
            return false;
        }
    }

    private bool isInitialized = false;

    // Start is called before the first frame update
    public void PlayGame() {
        StartCoroutine(StartGameDelay());
    }

    private IEnumerator StartGameDelay() {
        yield return new WaitForSeconds(1);
        Init();
        EnemyMissileSpawner.Instance.StartSpawning();
    }

    private void Init() {
        if (turretCollection == null) turretCollection = new List<Turret>();
        MissileCollection = new List<Missile>();
        DeadMissiles = new Queue<Missile>();

        EnemyMissileCollection = new List<Missile>();
        EnemyDeadMissiles = new Queue<Missile>();

        ExplosionCollection = new List<Explosion>();
        DeadExplosion = new Queue<Explosion>();

        EnemyExplosionCollection = new List<Explosion>();
        EnemyDeadExplosion = new Queue<Explosion>();

        isInitialized = true;
    }

    private void KillAll() {
        for (int i = 0; i < MissileCollection.Count; i++) {
            MissileCollection[i].Restart();
        }
        for (int i = 0; i < EnemyMissileCollection.Count; i++) {
            EnemyMissileCollection[i].Restart();
        }

        for (int i = 0; i < ExplosionCollection.Count; i++) {
            ExplosionCollection[i].Restart();
        }
        for (int i = 0; i < EnemyExplosionCollection.Count; i++) {
            EnemyExplosionCollection[i].Restart();
        }
    }

    public void Restart() {
        for (int i = 0; i < turretCollection.Count; i++) {
            turretCollection[i].Restart();
        }
        for (int i = 0; i < baseCollection.Count; i++) {
            baseCollection[i].Restart();
        }

        KillAll();
        StartCoroutine(StartGameDelay());
    }

    private bool isOkToTouch = true;
    private IEnumerator TouchCooldown() {
        yield return new WaitForSeconds(0.2f);
        isOkToTouch = true;
    }

    private void SetTouchCooldown() {
        isOkToTouch = false;
        StartCoroutine(TouchCooldown());
    }

    // Update is called once per frame
    private void Update() {
        if (!isInitialized) return;

        for (int touchIndex = Input.touchCount; --touchIndex >= 0;) {
            if (Input.touchCount == 1) {
                Touch touch = Input.GetTouch(touchIndex);
                if (TouchPhase.Began == touch.phase) {
                    FireMissile();
                }
            }
        }

        //if (Input.touchCount <= 1) {
        //    if (Input.GetMouseButtonDown(0)) {
        //        FireMissile();
        //    }
        //}


        //Handle Missiles
        HandleMissiles();
    }

    private void HandleMissiles() {
        for (int i = 0; i < MissileCollection.Count; i++) {
            Missile missile = MissileCollection[i];
            missile.HandleMissile();
        }
    }

    private void FireMissile() {
        //Sort the missile by distance to mouseclick
        turretCollection = turretCollection.OrderBy(_turret => Vector2.Distance(Utilities.ClickPosition, _turret.transform.position)).ToList();
        for (int i = 0; i < turretCollection.Count; i++) {
            Turret turret = turretCollection[i];
            if (turret.IsOkToFire && turret.IsAlive) {
                turret.FireMissile(Utilities.ClickPosition);
                break;
            }
        }
    }

    public Missile GetAMissile(Vector3 missilePosition, PlayerType playerType) {
        Missile deadMissile = null;
        if (playerType == PlayerType.Hero) {
            if (DeadMissiles.Count > 0) {
                deadMissile = DeadMissiles.Peek();
                DeadMissiles.Dequeue();
                deadMissile.gameObject.SetActive(true);
                deadMissile.IsAlive = true;
                deadMissile.playerType = playerType;
                deadMissile.SetMissilePosition(missilePosition);
                return deadMissile;
            }
        } else {
            if (EnemyDeadMissiles.Count > 0) {
                deadMissile = EnemyDeadMissiles.Peek();
                EnemyDeadMissiles.Dequeue();
                deadMissile.gameObject.SetActive(true);
                deadMissile.IsAlive = true;
                deadMissile.playerType = playerType;
                deadMissile.SetMissilePosition(missilePosition);
                return deadMissile;
            }
        }

        //If no available missile create a new one
        Missile newMissile = Instantiate(playerType == PlayerType.Hero ? GameConfig.Instance.missilePrefab : GameConfig.Instance.enemyMissilePrefab, playerType == PlayerType.Hero ? turretMissileParent : enemyMissileParent).GetComponent<Missile>();
        newMissile.playerType = playerType;
        newMissile.SetMissilePosition(missilePosition);
        return newMissile;
    }

    public Explosion GetAExplosion(Vector3 explosionPosition, PlayerType playerType) {
        Explosion deadExplosion = null;
        if (playerType == PlayerType.Hero) {
            if (DeadExplosion.Count > 0) {
                deadExplosion = DeadExplosion.Peek();
                DeadExplosion.Dequeue();
                deadExplosion.gameObject.SetActive(true);
                deadExplosion.IsAlive = true;
                return deadExplosion;
            }
        } else {
            if (EnemyDeadExplosion.Count > 0) {
                deadExplosion = EnemyDeadExplosion.Peek();
                EnemyDeadExplosion.Dequeue();
                deadExplosion.gameObject.SetActive(true);
                deadExplosion.IsAlive = true;
                return deadExplosion;
            }
        }

        //If no available missile create a new one
        Explosion newExplosion = Instantiate(playerType == PlayerType.Hero ? GameConfig.Instance.playerMissileExplosionPrefab : GameConfig.Instance.enemyMissileExplosionPrefab,
            playerType == PlayerType.Hero ? heroExplosionParent : enemyExplosionParent).GetComponent<Explosion>();
        if (playerType == PlayerType.Hero) {
            newExplosion.SetExplosion(GameConfig.Instance.missileExplosionDurationSeconds, GameConfig.Instance.missileExplosionSize, explosionPosition);
        } else {
            newExplosion.SetExplosion(GameConfig.Instance.enemyMissileExplosionDurationSeconds, GameConfig.Instance.enemyMissileExplosionSize, explosionPosition);
        }
        return newExplosion;
    }

    public Transform GetRandomTarget() {
        int dice = Random.Range(0, TargetCollection.Count);
        while (!TargetCollection[dice].GetComponent<Base>().IsAlive) {
            dice = Random.Range(0, TargetCollection.Count);
        }
        return TargetCollection[dice];
    }

}
