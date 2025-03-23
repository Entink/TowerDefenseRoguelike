using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public int maxUnits = 10;
    [SerializeField, ReadOnly] public int currentUnits = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public bool CanSpawnUnit()
    {
        return currentUnits < maxUnits;
    }

    public void RegisterUnit()
    {
        currentUnits++;
    }

    public void UnregisterUnit()
    {
        currentUnits--;
    }

    public void OnBaseDestroyed(bool playerBase)
    {
        if(playerBase)
        {
            Debug.Log("Lose");
        }
        else
        {
            Debug.Log("Win");
        }
    }
}
