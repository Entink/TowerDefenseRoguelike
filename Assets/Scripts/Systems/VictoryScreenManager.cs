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


    private void Start()
    {
        runData = FindAnyObjectByType<RunData>();
        continueButton.onClick.AddListener(OnContinue);

        int cash = RunResources.GetCash();
        int materials = RunResources.GetMaterials();

        int seed = MapRunData.currentSeed;
        int fightIndex = MapRunData.currentNode.id;

        var choices = ModifierChooser.ChooseOptions(modifierDb, 3, seed, fightIndex);

        Debug.Log("Wylosowane opcje:");
        foreach (var c in choices)
            Debug.Log($" - {c.displayName} ({c.rarity})");

        modifierChoicePanel.ShowOptions(choices, (chosen) =>
        {
            runData.AddModifier(chosen.id, chosen.defaultStacksOnPick);
            Debug.Log($"Wybrano: {chosen.displayName}");
            OnContinue();

        });

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
