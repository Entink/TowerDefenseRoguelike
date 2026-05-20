using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VictoryScreenManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI integrityText;
    
    [SerializeField] private RunData runData;
    [SerializeField] private RunModifierDatabase modifierDb;
    [SerializeField] private ModifierChoicePanel modifierChoicePanel;

    [SerializeField] private OnboardingVictory onboarding;

    [SerializeField] private RunSummaryPanel runSummaryPanel;
    [SerializeField] private FightSummaryPanel fightSummaryPanel;

    [SerializeField] private RewardConfig rewardConfig;

    public int cashReward;
    public int materialsReward;

    private bool isBossFight;
    private bool recoverableDefeat;

    private void Awake()
    {
        if (modifierDb) RunModifierLookup.SetDB(modifierDb);
    }


    private void Start()
    {
        Time.timeScale = 1f;

        if(continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinue);
        }

        var fight = BattleDataCarrier.selectedFight;
        isBossFight = fight != null && fight.isBoss;
        recoverableDefeat = FightResultCarrier.recoverableDefeat;

        runData = FindAnyObjectByType<RunData>();

        var cfg = rewardConfig != null ? rewardConfig : Resources.Load<RewardConfig>("RewardConfig");

        int difficulty = fight != null ? fight.difficulty : 1;

        bool fightWon = FightResultCarrier.fightWon;

        bool calculateRewards = fightWon || recoverableDefeat;

        var result = RewardCalculator.Compute(isBossFight, difficulty, calculateRewards, cfg);

        cashReward = Mathf.RoundToInt(result.cash * FightResultCarrier.cashMultiplier);
        materialsReward = Mathf.RoundToInt(result.materials * FightResultCarrier.materialsMultiplier);

        RunResources.AddCash(cashReward);
        RunStatsCollector.AddMaterials(materialsReward);

        RefreshResultText();

        var fightSnap = FightStatsCarrier.Last;
        bool tutorialActive = TutorialState.I != null && TutorialState.I.Active;

        if(!isBossFight)
        {
            if(fightSummaryPanel != null && !tutorialActive)
            {
                fightSummaryPanel.Show(fightSnap, materialsReward, RunStatsCollector.S.materialsEarned);
            }

            FightStatsCarrier.Clear();

            ShowModifierChoices();
        }
        else
        {
            if (modifierChoicePanel != null)
                modifierChoicePanel.gameObject.SetActive(false);

            if(fightSummaryPanel != null)
            {
                fightSummaryPanel.Show(fightSnap, materialsReward, RunStatsCollector.S.materialsEarned);
            }

            if(runSummaryPanel != null)
            {
                runSummaryPanel.Show("RUN SUMMARY (VICTORY)", -1);
            }

            if (continueButton != null)
                continueButton.gameObject.SetActive(true);
        }
    }

    private void ShowModifierChoices()
    {
        if(modifierChoicePanel == null || modifierDb == null)
        {
            if (continueButton != null)
                continueButton.gameObject.SetActive(true);

            return;
        }

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        int seed = MapRunData.currentSeed;

        int fightIndex = MapRunData.pendingNodeId;

        if (fightIndex < 0 && MapRunData.currentNode != null)
            fightIndex = MapRunData.currentNode.id;

        if (fightIndex < 0)
            fightIndex = 0;

        var choices = ModifierChooser.ChooseOptions(modifierDb, 3, seed, fightIndex);

        modifierChoicePanel.ShowOptions(choices, (chosen) =>
        {
            if (runData == null)
                runData = FindAnyObjectByType<RunData>();

            if (runData != null && chosen != null)
            {
                runData.AddModifier(chosen.id, chosen.defaultStacksOnPick);
                Debug.Log($"Wybrano: {chosen.displayName}");
            }

            if (onboarding != null)
                onboarding.CloseAfterPick();

            OnContinue();
        });
    }

    private void OnContinue()
    {
        bool tutorialActive = TutorialState.I != null && TutorialState.I.Active;

        if(tutorialActive)
        {
            TutorialState.I.Complete();
            PlayerPrefs.SetInt("finished_key", 1);

            RunData.ResetRun();
            MapRunData.Reset();
            RunSaveManager.Delete();
            RunStatsCollector.Reset();
            FightResultCarrier.Clear();

            SceneLoader.LoadScene("MainBaseScene");
            return;
        }

        bool fightIsBoss = BattleDataCarrier.selectedFight != null && BattleDataCarrier.selectedFight.isBoss;

        if(fightIsBoss)
        {
            Debug.Log("[VictoryScreenManager] Powrót do bazy");

            int payout = RunStatsCollector.S.materialsEarned;
            RunResources.AddMaterials(payout);
            RunStatsCollector.S.materialsEarned = 0;

            RunData.ResetRun();

            MapRunData.Reset();
            RunResources.Reset();
            RunSaveManager.Delete();
            RunStatsCollector.Reset();
            FightResultCarrier.Clear();

            SceneLoader.LoadScene("MainBaseScene");
        }
        else
        {
            if (MapRunData.currentNode != null)
                MapRunData.currentNode.wasVisisted = true;

            MapRunData.nodeToMarkVisited = MapRunData.pendingNodeId;
            MapRunData.pendingNodeId = -1;

            RunSaveManager.Save();
            FightResultCarrier.Clear();

            SceneLoader.LoadScene("MapScene");
        }
    }

    private void RefreshResultText()
    {
        if(resultText != null)
        {
            if (recoverableDefeat)
                resultText.text = "Fight Lost";
            else
                resultText.text = "Victory";
        }

        if(integrityText != null)
        {
            if(recoverableDefeat)
            {
                integrityText.gameObject.SetActive(true);
                integrityText.text = $"Base Integrity decreased\n{FightResultCarrier.integrityText}";
            }
            else
            {
                integrityText.text = "";
                integrityText.gameObject.SetActive(false);
            }
        }

        if(cashText != null)
        {
            cashText.text = $"Cash earned: {cashReward}\nMaterials earned: {materialsReward}\nTotal cash: {RunResources.GetCash()}";
        }
    }
}
