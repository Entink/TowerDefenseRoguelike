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

    [SerializeField] private MapEventDatabase eventDb;
    [SerializeField] private MapEventPanel eventPanel;




    public void Setup(MapNodeData nodeData, MapManager manager, MapEventDatabase eventDb, MapEventPanel eventPanel)
    {
        data = nodeData;
        mapManager = manager;
        this.eventDb = eventDb;
        this.eventPanel = eventPanel;

        label.text = data.type.ToString();

        UpdateVisual();

        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {

        if (data.type == NodeType.Fight || data.type == NodeType.Boss)
        {
            mapManager.SelectNode(this);


            var fight = FightDatabase.instance.GetByTypeAndIndex(data.type, data.fightIndex);
            if (fight == null)
            {
                Debug.LogError($"Fight not found for {data.type} index={data.fightIndex}");
                return;
            }

            BattleDataCarrier.selectedFight = fight;
            SceneLoader.LoadScene("LoadingScene");
        }
        else if (data.type == NodeType.Shop)
        {
            mapManager.SelectNode(this);

            SceneLoader.LoadScene("ShopScene");
        }
        else if (data.type == NodeType.Event)
        {

            if (eventDb == null || eventPanel == null)
            {
                Debug.LogWarning("Event click: no eventDb or eventPanel in MapNodeUI.");
                return;
            }

            var rng = new System.Random(unchecked(MapRunData.currentSeed ^ (data.id * 73856093)));

            MapEventDef def = eventDb.PickForColumn(rng, data.columnIndex);
            

            if(def == null)
            {
                RunResources.AddMaterials(10);
                mapManager.SelectNode(this);
                return;
            }

            eventPanel.gameObject.SetActive(true);
            eventPanel.Show(def, onResolved: () => { mapManager.SelectNode(this); });

        }
        else
        {
            mapManager.SelectNode(this);

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
        if (data.wasVisisted)
        {
            GetComponent<Image>().color = Color.gray;
        }
        else
        {
            GetComponent<Image>().color = Color.white;

        }
    }

    public void DrawConnectionTo(MapNodeUI target)
    {
        GameObject lineGO = new GameObject("Line", typeof(LineRenderer));
        lineGO.transform.SetParent(MapManager.instance.lineContainer, false);

        LineRenderer lr = lineGO.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(1f, 1f, 1f, 0.5f);
        lr.endColor = new Color(1f, 1f, 1f, 0.5f);
        lr.sortingOrder = -1;
        lr.useWorldSpace = false;

        RectTransform fromRT = GetComponent<RectTransform>();
        RectTransform toRT = target.GetComponent<RectTransform>();

        var lc = MapManager.instance.lineContainer;
        Vector3 fromLocal = lc.InverseTransformPoint(fromRT.TransformPoint(fromRT.rect.center));
        Vector3 toLocal = lc.InverseTransformPoint(toRT.TransformPoint(toRT.rect.center));

        lr.SetPosition(0, fromLocal);
        lr.SetPosition(1, toLocal);
    }

    public Vector3 GetWorldCenter(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return (corners[0] + corners[2]) / 2f;
    }


}