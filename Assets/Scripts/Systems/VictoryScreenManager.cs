using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VictoryScreenManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI materialsText;
    
    [SerializeField] private RunData runData;
    [SerializeField] private RunModifierDatabase modifierDb;
    [SerializeField] private ModifierChoicePanel modifierChoicePanel;

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

        int cash = RunResources.GetCash();
        int materials = RunResources.GetMaterials();

        if(!isBossFight)
        {
            int seed = MapRunData.currentSeed;
            int fightIndex = MapRunData.currentNode.id;

            var choices = ModifierChooser.ChooseOptions(modifierDb, 3, seed, fightIndex);

            modifierChoicePanel.ShowOptions(choices, (chosen) =>
            {
                runData.AddModifier(chosen.id, chosen.defaultStacksOnPick);
                Debug.Log($"Wybrano: {chosen.displayName}");
                OnContinue();

            });
        }
        else
        {
            if (modifierChoicePanel != null) modifierChoicePanel.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
        

        RunResources.AddCash(cashReward);
        RunResources.AddMaterials(materialsReward);
        Debug.Log($"Cash: {RunResources.GetCash()} | Materials: {RunResources.GetMaterials()}");
        cashText.text = $"Cash earned: {cashReward}\nTotal cash: {RunResources.GetCash()}";
        materialsText.text = $"Materials earned: {materialsReward}\nTotal materials: {RunResources.GetMaterials()}";
            
    }

    private void OnContinue()
    {
        if(MapRunData.currentNode.type == NodeType.Boss)
        {
            
            RunData.ResetRun();
            
            MapRunData.Reset();
            RunResources.Reset();


            SceneLoader.LoadScene("MainBaseScene");
        }
        else
        {
            SceneLoader.LoadScene("MapScene");

        }

    }
}
