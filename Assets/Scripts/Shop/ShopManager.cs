using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ShopPanel shopPanel;

    private void Start()
    {
        int col = MapRunData.currentNode != null ? MapRunData.currentNode.columnIndex : 0;
        //FindObjectOfType<ShopPanel>().Open(col, onExit: () => SceneLoader.LoadScene("MapScene"));

        shopPanel.gameObject.SetActive(true);
        shopPanel.Open(col, onExit: () => SceneLoader.LoadScene("MapScene"));

        
    }

    
}