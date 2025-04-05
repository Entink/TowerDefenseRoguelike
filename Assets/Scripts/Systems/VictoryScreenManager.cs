using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI materialsText;

    public int cashReward;
    public int materialsReward;


    private void Start()
    {
        continueButton.onClick.AddListener(OnContinue);

        int cash = RunResources.GetCash();
        int materials = RunResources.GetMaterials();

        RunResources.AddCash(cashReward);
        RunResources.AddMaterials(materialsReward);
        Debug.Log($"Cash: {RunResources.GetCash()} | Materials: {RunResources.GetMaterials()}");
        cashText.text = $"Cash earned: {cashReward}\nTotal cash: {RunResources.GetCash()}";
        materialsText.text = $"Materials earned: {materialsReward}\nTotal materials: {RunResources.GetMaterials()}";
            
    }

    private void OnContinue()
    {
        SceneLoader.LoadScene("MapScene");
    }
}
