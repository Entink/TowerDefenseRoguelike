using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    private void Start()
    {
        UpdateGold(CurrencyManager.instance.GetGold());


        CurrencyManager.instance.OnGoldChanged.AddListener(UpdateGold);
    }

    private void UpdateGold(int amount)
    {
        goldText.text = $"Gold: {amount}";
    }
}
