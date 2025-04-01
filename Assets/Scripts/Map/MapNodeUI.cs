using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class MapNodeUI : MonoBehaviour
{
    public TextMeshProUGUI label;
    public MapNodeData data;
    private MapManager mapManager;

    private List<MapNodeUI> connectedNodes = new List<MapNodeUI>();


    public void Setup(MapNodeData nodeData, MapManager manager)
    {
        data = nodeData;
        mapManager = manager;
        label.text = data.type.ToString();

        UpdateVisual();

        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        mapManager.SelectNode(this);

        if (data.type == NodeType.Fight || data.type == NodeType.Boss)
        {
            BattleDataCarrier.selectedFight = FightDatabase.instance.allFights[data.fightIndex];
            SceneLoader.LoadScene("LoadingScene");
        }
    }

    public void AddConnection(MapNodeUI target)
    {
        connectedNodes.Add(target);
    }

    public List<MapNodeUI> GetConnectedNodes()
    {
        return connectedNodes;
    }

    public void SetInteractable(bool value)
    {
        GetComponent<Button>().interactable = value;
    }

    public MapNodeData GetNodeData()
    {
        return data;
    }

    public void UpdateVisual()
    {
        if(data.wasVisisted)
        {
            GetComponent<Image>().color = Color.gray;
        }
        else
        {
            GetComponent<Image>().color = Color.white;

        }
    }

}
