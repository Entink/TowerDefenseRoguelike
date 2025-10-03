using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopOfferButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Button button;
    [SerializeField] Image bg;

    ShopOfferDef data;
    System.Action<ShopOfferDef> onBuy;

    public void Setup(ShopOfferDef def, System.Action<ShopOfferDef> onBuy)
    {
        data = def;
        this.onBuy = onBuy;

        if (icon) icon.sprite = def.icon;
        if (titleText) titleText.text = def.title;
        if (descText) descText.text = def.description;
        if (bg) bg.color = def.tint;

        string price = def.currency switch
        {
            ShopCurrency.Cash => $"{def.priceCash}$",
            ShopCurrency.Materials => $"{def.priceMaterials} materials",
            ShopCurrency.Both => $"{def.priceCash}$ + {def.priceMaterials} materials",
            _ => ""
        };

        if (priceText) priceText.text = price;

        int haveCash = RunResources.GetCash();
        int haveMat = RunResources.GetMaterials();
        bool okCash = haveCash >= def.priceCash || def.currency == ShopCurrency.Materials;
        bool okMat = haveMat >= def.priceMaterials || def.currency == ShopCurrency.Cash;
        bool canBuy = def.currency == ShopCurrency.Both
            ? (haveCash >= def.priceCash && haveMat >= def.priceMaterials)
            : (def.currency == ShopCurrency.Cash
                ? haveCash >= def.priceCash
                : haveMat >= def.priceMaterials);


        if(!canBuy && descText != null)
        {
            string need = (!okCash && !okMat)
                ? "Not enough cash and materials"
                : (!okCash
                    ? "Not enough cash"
                    : "Not enough materials");
            descText.text = string.IsNullOrWhiteSpace(def.description)
                ? $"<color=#FF5555>{need}</color>"
                : $"{def.description}\n<size=80%><color=#FF5555>{need}</color></size>";
        }


        if(button)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = canBuy;
            button.onClick.AddListener(() => this.onBuy?.Invoke(def));
        }

    }
}