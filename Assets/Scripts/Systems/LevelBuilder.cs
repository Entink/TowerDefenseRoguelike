using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public Transform floor;
    public Transform playerBase;
    public Transform enemyBase;

    public FightData testFight;
    [SerializeField] private FightData fight;


    public float baseOffsetFromEdge = 1f;

    private void Awake()
    {
        fight = BattleDataCarrier.selectedFight != null ? BattleDataCarrier.selectedFight : testFight;
        if(fight == null)
        {
            Debug.LogError("Brak danych o walce!");
            return;
        }

        float length = fight.levelLenght;

        floor.localScale = new Vector3(length, floor.localScale.y, floor.localScale.z);

        float halfLength = length / 2f;

        playerBase.localPosition = new Vector3(halfLength - baseOffsetFromEdge, playerBase.localPosition.y, 0);
        enemyBase.localPosition = new Vector3(-halfLength + baseOffsetFromEdge, enemyBase.localPosition.y, 0);

    }
}
