using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class MapNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI label;
    public MapNodeData data;
    private MapManager mapManager;

    private readonly List<MapNodeUI> connectedNodes = new List<MapNodeUI>();

    [SerializeField] private MapEventDatabase eventDb;
    [SerializeField] private MapEventPanel eventPanel;

    [SerializeField] private Color connectionNormalColor = new Color(0.55f, 0.55f, 0.55f, 1f);
    [SerializeField] private Color connectionHighlightColor = Color.white;

    private readonly List<Image> outgoingConnectionImages = new();

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
                    MapRunData.pendingNodeId = data.id;


                    var fight = FightDatabase.instance.GetByTypeAndIndex(data.type, data.fightIndex);
                    if (fight == null)
                    {
                        Debug.LogError($"Fight not found for {data.type} index={data.fightIndex}");
                        return;
                    }

                    BattleDataCarrier.selectedFight = fight;
                    RunSaveManager.Save();
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
            GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
        else
        {
            GetComponent<Image>().color = Color.white;

        }
    }

    public void DrawConnectionTo(MapNodeUI target)
    {
        RectTransform nodeContainer = MapManager.instance.GetNodeContainerRect();
        RectTransform lineContainer = MapManager.instance.lineContainer;

        RectTransform fromRT = GetComponent<RectTransform>();
        RectTransform toRT = target.GetComponent<RectTransform>();

        Vector2 fromLocal = GetLocalCenterInContainer(fromRT, nodeContainer);
        Vector2 toLocal = GetLocalCenterInContainer(toRT, nodeContainer);

        Vector2 direction = toLocal - fromLocal;
        float length = direction.magnitude;

        RectTransform prefab = MapManager.instance.ConnectionPrefab;
        RectTransform lineRT = Instantiate(prefab, lineContainer, false);
        lineRT.gameObject.name = "Line";
        lineRT.SetAsFirstSibling();

        Image lineImage = lineRT.GetComponent<Image>();
        lineImage.color = connectionNormalColor;
        outgoingConnectionImages.Add(lineImage);

        lineRT.anchorMin = new Vector2(0f, 0.5f);
        lineRT.anchorMax = new Vector2(0f, 0.5f);
        lineRT.pivot = new Vector2(0.5f, 0.5f);

        Vector2 size = lineRT.sizeDelta;
        size.x = length;
        lineRT.sizeDelta = size;

        lineRT.anchoredPosition = (fromLocal + toLocal) * 0.5f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRT.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public Vector3 GetWorldCenter(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return (corners[0] + corners[2]) / 2f;
    }

    private Vector2 GetLocalCenterInContainer(RectTransform target, RectTransform container)
    {
        Vector3 worldCenter = target.TransformPoint(target.rect.center);
        Vector3 localPoint3 = container.InverseTransformPoint(worldCenter);
        return localPoint3;
    }

    
    private void SetOutgoingConnectionsColor(Color color)
    {
        for(int i = outgoingConnectionImages.Count - 1; i >= 0; i--)
        {
            if(outgoingConnectionImages[i] == null)
            {
                outgoingConnectionImages.RemoveAt(i);
                continue;
            }

            outgoingConnectionImages[i].color = color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOutgoingConnectionsColor(connectionHighlightColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOutgoingConnectionsColor(connectionNormalColor);
    }
}