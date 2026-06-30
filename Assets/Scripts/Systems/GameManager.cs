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

    [SerializeField] private RewardConfig rewardConfig;
    private bool defeatProcessed;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        defeatProcessed = false;
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
        if(DebugCombatSession.ReturnToBaseAfterFight)
        {
            DebugCombatSession.Clear();
            SceneLoader.LoadScene("MainBaseScene");
            return;
        }

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

        FightResultCarrier.SetVictory();
        SceneLoader.LoadScene("VictoryScene");
    }

    private void OnPlayerDefeated()
    {
        if (defeatProcessed) return;
        defeatProcessed = true;

        var fight = BattleDataCarrier.selectedFight;
        bool isBossFight = fight != null && fight.isBoss;

        if(BaseIntegrityManager.I != null && BaseIntegrityManager.I.CanLoseMoreIntegrity() && !isBossFight)
        {
            bool emergencyTriggered =
                RunData.I != null &&
                RunData.I.TryConsumeModifierStack(RunModifierId.EmergencyProtocol);

            if (!emergencyTriggered)
                BaseIntegrityManager.I.TryApplyLoss();

            var snap = CombatStatsTracker.I?.ToSnapshot();
            FightStatsCarrier.Set(snap);

            if(snap != null)
            {
                RunStatsCollector.AddFromFightSnapshot(snap);
                RunStatsCollector.OnFightResult(false);
            }
            else
            {
                RunStatsCollector.AddFromFight(CombatStatsTracker.I);
                RunStatsCollector.OnFightResult(false);
            }

            

            MapRunData.nodeToMarkVisited = MapRunData.pendingNodeId;


            RunSaveManager.Save();

            string resultHeader = emergencyTriggered ? "Emergency Protocol activated" : "Base Integrity decreased";

            FightResultCarrier.SetRecoverableDefeat(BaseIntegrityManager.I.GetDisplayText(), resultHeader);

            if(emergencyTriggered)
            {
                Debug.Log($"[EmergencyProtocol] Prevented integrity loss. CUrrent: {BaseIntegrityManager.I.GetDisplayText()}");
            }
            else
            {
                Debug.Log($"[BaseIntegrity] Fight lost. Integrity lowered to {BaseIntegrityManager.I.GetDisplayText()}");
            }


            SceneLoader.LoadScene("VictoryScene");
            return;
        }

        runState = RunState.Lost;
        RunStatsCollector.AddFromFight(CombatStatsTracker.I);
        RunStatsCollector.OnFightResult(false);

        var cfg = rewardConfig != null ? rewardConfig : Resources.Load<RewardConfig>("RewardConfig");



        int totalMaterials = RunStatsCollector.S.materialsEarned;
        int payoutMaterials = Mathf.RoundToInt(totalMaterials * 0.7f);

        Debug.Log($"[DEFEAT MATERIALS] RunStatsCollector={RunStatsCollector.S.materialsEarned}, RunResources={RunResources.GetMaterials()}");

        DefeatPayoutCarrier.materials = payoutMaterials;
        RunStatsCollector.S.materialsEarned = 0;

        runSummaryPanel.Show("RUN SUMMARY (DEFEAT)", payoutMaterials);


        Time.timeScale = 0f;

        if (defeatScreen != null) defeatScreen.SetActive(true);

        
    }

    
}
