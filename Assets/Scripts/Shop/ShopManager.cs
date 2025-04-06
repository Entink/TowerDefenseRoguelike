using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI cashText;
    public Button[] itemButtons;
    public Button returnButton;
    public ShopItem[] availableItems;

    private void Start()
    {

        RunResources.AddCash(49);
        UpdateCashDisplay();

        for(int i = 0; i < itemButtons.Length && i < availableItems.Length; i++)
        {
            int index = i;
            TextMeshProUGUI btnText = itemButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = $"{availableItems[i].name}\n({availableItems[i].cost}$)";

            availableItems[i].effect = new BonusHPItem(2f);
            itemButtons[i].onClick.AddListener(() => TryBuy(index));
        }

        returnButton.onClick.AddListener(() => ReturnToMap());
    }

    private void UpdateCashDisplay()
    {
        cashText.text = $"Cash: {RunResources.GetCash()}";
    }

    private void TryBuy(int index)
    {
        int cost = availableItems[index].cost;
        if(RunResources.GetCash() >= cost)
        {
            RunResources.AddCash(-cost);
            RunData.AddShopItem(availableItems[index]);
            Debug.Log($"Zakupiono: {availableItems[index].name} - {availableItems[index].description}");
            UpdateCashDisplay();
            itemButtons[index].interactable = false;
            
        }
        else
        {
            Debug.LogWarning("Za ma³o cash!");
        }
    }

    private void ReturnToMap()
    {
        SceneLoader.LoadScene("MapScene");
    }
}
