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

    private readonly List<MapNodeUI> connectedNodes = new List<MapNodeUI>();

    [SerializeField] private MapEventDatabase eventDb;
    [SerializeField] private MapEventPanel eventPanel;

    [SerializeField] private Color connectionStartColor;
    [SerializeField] private Color connectionEndColor;


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
        switch (data.type)
        {
            case NodeType.Fight:
            case NodeType.Boss:
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
                    break;
            }

            case NodeType.Shop:
            {
                    mapManager.SelectNode(this);
                    SceneLoader.LoadScene("ShopScene");
                    break;
            }


            case NodeType.Event:
            {
                    EventFlags.Load();

                    var candidates = eventDb.events.Where(def =>
                    {
                        if (def == null) return false;

                        if (def.hideAfterResolve && !string.IsNullOrEmpty(def.eventId) && EventFlags.IsResolved(def.eventId))
                            return false;

                        bool hasUnlockOption = def.options != null && def.options.Any(o => o.unlocksUnit);
                        if (hasUnlockOption)
                        {
                            bool allUnlockAlreadyOwned = def.options
                                .Where(o => o.unlocksUnit)
                                .All(o => UnitUnlocks.IsUnlocked(o.unitToUnlock));

                            if (allUnlockAlreadyOwned)
                                return false;


                        }

                        if (data.columnIndex >= 0)
                        {
                            if (data.columnIndex < def.minColumn || data.columnIndex > def.maxColumn)
                                return false;
                        }

                        return true;
                    }).ToList();

                    if (candidates.Count == 0)
                    {
                        Debug.Log("[MapNodeUI] No available events for this node.");

                        mapManager.SelectNode(this);

                        return;
                    }

                    var rng = new System.Random(unchecked(MapRunData.currentSeed ^ (data.id * 5341265)));
                    var def = candidates[rng.Next(candidates.Count)];

                    eventPanel.gameObject.SetActive(true);
                    eventPanel.Show(def, onResolved: () => mapManager.SelectNode(this));
                    break;
            }

            default:
            {
                mapManager.SelectNode(this);
                break;
            }

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
        lr.startColor = connectionStartColor;
        lr.endColor = connectionEndColor;
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