using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public TextMeshProUGUI limitText;
    public TextMeshProUGUI costText;
    public int baseCost = 5;

    [SerializeField] private static int baseUnitLimit = 3;
    private int Level => PlayerPrefs.GetInt("UnitLimitLevel", 0);
    private int currentLimit => baseUnitLimit + Level;
    private int nextUpgradeCost => baseCost + (Level * 10);

    private void Start()
    {
        UpdateUI();
    }

    public void TryUpgradeLimit()
    {
        if(RunResources.GetMaterials() >= nextUpgradeCost)
        {
            RunResources.AddMaterials(-nextUpgradeCost);
            PlayerPrefs.SetInt("UnitLimitLevel", Level + 1);
            PlayerPrefs.Save();

            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough materials");
        }
    }

    private void UpdateUI()
    {
        limitText.text = $"Unit Limit: {currentLimit}";
        costText.text = $"Unit Limit Upgrade Cost: {nextUpgradeCost}";
    }

    public static int GetCurrentUnitLimit()
    {
        return 3 + PlayerPrefs.GetInt("UnitLimitLevel", 0);
    }
}
