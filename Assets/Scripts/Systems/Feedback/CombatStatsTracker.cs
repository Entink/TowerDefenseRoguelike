using UnityEngine;

public class CombatStatsTracker : MonoBehaviour
{
    public static CombatStatsTracker I { get; private set; }

    public float fightTime;
    public int unitsSpawned;
    public int enemiesKilled;
    public int playerUnitsKilled;
    public int goldSpent;
    public float damageDealt;
    public float damageTaken;

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        fightTime += Time.deltaTime;
    }

    public void OnUnitSpawned() { unitsSpawned++; }
    public void OnEnemyKilled() { enemiesKilled++; }
    public void OnPlayerUnitKilled() { playerUnitsKilled++; }
    public void OnGoldSpent(int amount) { goldSpent += amount; }
    public void OnDamageDealt(float amount) { damageDealt += amount; }
    public void OnDamageTaken(float amount) { damageTaken += amount; }

    public CombatStatsSnapshot ToSnapshot()
    {
        return new CombatStatsSnapshot
        {
            fightTime = fightTime,
            unitsSpawned = unitsSpawned,
            enemiesKilled = enemiesKilled,
            playerUnitsKilled = playerUnitsKilled,
            goldSpent = goldSpent,
            damageDealt = damageDealt,
            damageTaken = damageTaken,
        };
    }
}
