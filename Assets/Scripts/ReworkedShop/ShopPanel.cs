using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ShopPanel : MonoBehaviour
{
    [Header("Database & UI")]
    [SerializeField] ShopDatabase databse;
    [SerializeField] Transform offersContainer;
    [SerializeField] ShopOfferButton offerPrefab;
    [SerializeField] Button rerollButon;
    [SerializeField] TextMeshProUGUI rerollPriceText;
    [SerializeField] Button closeButton;

    [Header("Config")]
    [SerializeField] int offersCount = 3;
    [SerializeField] int baseRerollCost = 20;
    [SerializeField] float rerollCostScale = 1.5f;

    int rerollUsed = 0;
    List<ShopOfferDef> currentOffers = new();
    HashSet<string> boughtUniques = new();
    int columnIndexForThisShop = 0;

    Action onExit;

    private void Awake()
    {
        if (rerollButon) rerollButon.onClick.AddListener(Reroll);
        if (closeButton) closeButton.onClick.AddListener(() => onExit?.Invoke());
        gameObject.SetActive(false);
    }

    public void Open(int columnIndex, Action onExit)
    {
        this.onExit = onExit;
        columnIndexForThisShop = columnIndex;

        GenerateOffers();
        UpdateRerollUI();
        gameObject.SetActive(true);
    }

    void GenerateOffers()
    {
        foreach (Transform ch in offersContainer) Destroy(ch.gameObject);
        currentOffers.Clear();

        var rng = new System.Random(unchecked(MapRunData.currentSeed ^ (columnIndexForThisShop * 37615412) ^ rerollUsed));
        var picks = databse.Pick(rng, columnIndexForThisShop, offersCount, boughtUniques);

        foreach(var def in picks)
        {
            currentOffers.Add(def);
            var ui = Instantiate(offerPrefab, offersContainer);
            ui.Setup(def, TryBuy);
        }

        Canvas.ForceUpdateCanvases();
    }

    void TryBuy(ShopOfferDef def)
    {
        bool canBuy = false;

        switch(def.currency)
        {
            case ShopCurrency.Cash:
                canBuy = RunResources.GetCash() >= def.priceCash;
                break;

            case ShopCurrency.Materials:
                canBuy = RunResources.GetMaterials() >= def.priceMaterials;
                break;

            case ShopCurrency.Both:
                canBuy = RunResources.GetCash() >= def.priceCash && RunResources.GetMaterials() >= def.priceMaterials;
                break;
        }

        if (!canBuy) return;

        if (def.currency == ShopCurrency.Cash || def.currency == ShopCurrency.Both)
            RunResources.AddCash(-def.priceCash);

        if (def.currency == ShopCurrency.Materials || def.currency == ShopCurrency.Both)
            RunResources.AddMaterials(-def.priceMaterials);

        if (def.deltaCash != 0) RunResources.AddCash(def.deltaCash);
        if (def.deltaMaterials != 0) RunResources.AddMaterials(def.deltaMaterials);
        if (def.addModifier) RunData.I.AddModifier(def.modifier, def.modifierStacks);

        if (def.uniquePerRun) boughtUniques.Add(def.name);

        GenerateOffers();
    }

    void Reroll()
    {
        int cost = GetCurrentRerollCost();
        if (RunResources.GetCash() < cost) return;

        RunResources.AddCash(-cost);
        rerollUsed++;
        GenerateOffers();
        UpdateRerollUI();
    }

    int GetCurrentRerollCost()
    {
        float cost = baseRerollCost;
        for (int i = 0; i < rerollUsed; i++)
            cost *= rerollCostScale;

        return Mathf.RoundToInt(cost);
    }

    void UpdateRerollUI()
    {
        int cost = GetCurrentRerollCost();
        if (rerollPriceText) rerollPriceText.text = $"Reroll: {cost}";
        if (rerollButon) rerollButon.interactable = RunResources.GetCash() >= cost;
    }
}