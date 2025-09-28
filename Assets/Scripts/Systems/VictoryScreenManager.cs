using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI materialsText;

    [SerializeField] private RunData runData;

    public int cashReward;
    public int materialsReward;


    private void Start()
    {
        runData = FindAnyObjectByType<RunData>();
        continueButton.onClick.AddListener(OnContinue);

        int cash = RunResources.GetCash();
        int materials = RunResources.GetMaterials();

        runData.AddModifier(RunModifierId.RecruitmentDrive, 1);
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
