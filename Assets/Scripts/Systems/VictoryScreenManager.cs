using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VictoryScreenManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI cashText;
    
    [SerializeField] private RunData runData;
    [SerializeField] private RunModifierDatabase modifierDb;
    [SerializeField] private ModifierChoicePanel modifierChoicePanel;

    [SerializeField] private OnboardingVictory onboarding;

    [SerializeField] private RunSummaryPanel runSummaryPanel;
    [SerializeField] private FightSummaryPanel fightSummaryPanel;

    public int cashReward;
    public int materialsReward;

    private void Awake()
    {
        if (modifierDb) RunModifierLookup.SetDB(modifierDb);
    }


    private void Start()
    {
        
        bool isBossFight = false;
        if (BattleDataCarrier.selectedFight != null)
            isBossFight = BattleDataCarrier.selectedFight.isBoss;

        runData = FindAnyObjectByType<RunData>();
        continueButton.onClick.AddListener(OnContinue);

        RunResources.AddCash(cashReward);
        RunStatsCollector.AddMaterials(materialsReward);

        //Debug.Log($"Cash: {RunResources.GetCash()} | Materials: {RunResources.GetMaterials()}");
        cashText.text = $"Cash earned: {cashReward}\nTotal cash: {RunResources.GetCash()}";

        var fightSnap = FightStatsCarrier.Last;

        if (!isBossFight)
        {
            if (fightSummaryPanel && !TutorialState.I.Active)
                fightSummaryPanel.Show(fightSnap, materialsReward, RunStatsCollector.S.materialsEarned);

            FightStatsCarrier.Clear();

            int seed = MapRunData.currentSeed;
            int fightIndex = MapRunData.currentNode.id;

            var choices = ModifierChooser.ChooseOptions(modifierDb, 3, seed, fightIndex);

            modifierChoicePanel.ShowOptions(choices, (chosen) =>
            {
                runData.AddModifier(chosen.id, chosen.defaultStacksOnPick);
                if (onboarding) onboarding.CloseAfterPick();
                Debug.Log($"Wybrano: {chosen.displayName}");
                OnContinue();

            });
        }
        else
        {
            if (modifierChoicePanel != null) modifierChoicePanel.gameObject.SetActive(false);
            fightSummaryPanel.Show(fightSnap, materialsReward, RunStatsCollector.S.materialsEarned);

            //fightSummaryPanel.gameObject.SetActive(false);

            if (runSummaryPanel) runSummaryPanel.Show("RUN SUMMARY (VICTORY)", -1);

            continueButton.gameObject.SetActive(true);
        }

        
        

        
            
    }

    private void OnContinue()
    {
        if(TutorialState.I != null && TutorialState.I.Active)
        {
            TutorialState.I.Complete();
            PlayerPrefs.SetInt("finished_key", 1);
            RunData.ResetRun();
            MapRunData.Reset();
            RunSaveManager.Delete();
            RunStatsCollector.Reset();

            SceneLoader.LoadScene("MainBaseScene");
            return;
        }

        if(BattleDataCarrier.selectedFight.isBoss)
        {
            Debug.Log("[VictoryScreenManager] Powrót do bazy");
            int payout = RunStatsCollector.S.materialsEarned;
            RunResources.AddMaterials(payout);

            RunData.ResetRun();
            
            MapRunData.Reset();
            RunResources.Reset();
            RunSaveManager.Delete();
            RunStatsCollector.Reset();

            SceneLoader.LoadScene("MainBaseScene");
        }
        else
        {
            if (MapRunData.currentNode != null)
                MapRunData.currentNode.wasVisisted = true;
            MapRunData.nodeToMarkVisited = MapRunData.pendingNodeId;
            MapRunData.pendingNodeId = -1;
            RunSaveManager.Save();
            SceneLoader.LoadScene("MapScene");

        }

    }
}
