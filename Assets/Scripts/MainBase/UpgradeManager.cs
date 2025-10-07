using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public TextMeshProUGUI limitText;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        limitText.text = $"Unit Limit: {GetCurrentUnitLimit()}";
    }

    public static int GetCurrentUnitLimit()
    {
        int baseLimit = 3;
        int bonus = 0;
        return baseLimit + bonus;
    }
}
