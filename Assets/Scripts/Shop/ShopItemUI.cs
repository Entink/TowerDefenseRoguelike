using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button buyButton;

    private ShopItemData itemData;

    public void Setup(ShopItemData data)
    {
        itemData = data;
        nameText.text = data.itemName;
        descriptionText.text = data.description;
        costText.text = $"{data.cost}$";

        buyButton.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        if(RunResources.GetCash() >= itemData.cost)
        {
            RunResources.AddCash(-itemData.cost);
            RunData.AddShopItem(itemData);

            itemData.ApplyEffect();

            Destroy(gameObject);
        }
    }
}