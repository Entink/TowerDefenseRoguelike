using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Units limit")]
    [SerializeField] int baseMaxUnits = 10;
    public int currentMaxUnits;

    [SerializeField] RunData runData;

    [SerializeField, ReadOnly] public int currentUnits = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    void OnEnable()
    {
        if (runData == null) runData = RunData.I;
        if (runData != null) runData.OnModifiersChanged += RecalculateLimits;
        RecalculateLimits();
    }

    void OnDisable()
    {
        if (runData != null) runData.OnModifiersChanged -= RecalculateLimits;
    }

    void RecalculateLimits()
    {
        currentMaxUnits = baseMaxUnits;
        if(runData != null)
        {
            RunModifiers.ApplyLimits(runData.activeModifiers, ref currentMaxUnits);
        }
    }

    public int GetMaxUnits() => currentMaxUnits;

    public bool CanSpawnUnit()
    {
        return currentUnits < currentMaxUnits;
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
            SceneLoader.LoadScene("MapScene");

        }
        else
        {
            
            Debug.Log("Win");
            SceneLoader.LoadScene("VictoryScene");
        }
    }

}
