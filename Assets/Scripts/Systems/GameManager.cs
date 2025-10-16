using UnityEngine;

public enum RunState
{
    InFight,
    Won,
    Lost
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    [SerializeField] private GameObject defeatScreen;

    [Header("State")]
    [SerializeField] private RunState runState = RunState.InFight;
    private bool endTriggered = false;



    [Header("Units limit")]
    [SerializeField] int baseMaxUnits = 10;
    public int currentMaxUnits;

    [SerializeField] RunData runData;

    [SerializeField, ReadOnly] public int currentUnits = 0;

    [SerializeField] RunSummaryPanel runSummaryPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        runState = RunState.InFight;
        endTriggered = false;
        if (defeatScreen != null) defeatScreen.SetActive(false);
    }

    public bool IsInputAllowed() => runState == RunState.InFight && !endTriggered;

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
            OnPlayerDefeated();

        }
        else
        {
            OnEnemyDefetead();
            
        }
    }

    private void OnEnemyDefetead()
    {
        runState = RunState.Won;

        var snap = CombatStatsTracker.I?.ToSnapshot();
        FightStatsCarrier.Set(snap);

        if(snap != null)
        {
            RunStatsCollector.AddFromFightSnapshot(snap);
            RunStatsCollector.OnFightResult(true);
        }

        Debug.Log("Win");
        SceneLoader.LoadScene("VictoryScene");
    }

    private void OnPlayerDefeated()
    {
        runState = RunState.Lost;
        RunStatsCollector.AddFromFight(CombatStatsTracker.I);
        RunStatsCollector.OnFightResult(false);

        int bank = RunStatsCollector.S.materialsEarned;
        int payout = Mathf.RoundToInt(bank * 0.7f);
        runSummaryPanel.Show("RUN SUMMARY (DEFEAT)", payout);
        PlayerPrefs.SetInt("defeat_payout_materials", payout);
        PlayerPrefs.Save();

        Time.timeScale = 0f;

        if (defeatScreen != null) defeatScreen.SetActive(true);

        
    }

    
}
