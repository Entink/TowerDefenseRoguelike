using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItemData> itemPool;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private int itemsToShow = 3;

    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private Button returnButton;

    private void Start()
    {
        returnButton.onClick.AddListener(ReturnToMap);
        List<ShopItemData> chosen = new();

        while (chosen.Count < itemsToShow)
        {
            var item = itemPool[Random.Range(0, itemPool.Count)];

            if(!chosen.Contains(item))
            {
                chosen.Add(item);
            }
        }

        foreach (var item in chosen)
        {
            GameObject go = Instantiate(itemUIPrefab, container);
            go.GetComponent<ShopItemUI>().Setup(item);
        }
    }

    private void FixedUpdate()
    {
        cashText.text = $"Cash: {RunResources.GetCash()}$";
    }

    private void ReturnToMap()
    {
        SceneLoader.LoadScene("MapScene");
    }
}