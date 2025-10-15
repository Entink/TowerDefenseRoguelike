using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;


    [Header("UI")]
    [SerializeField] private ScrollRect mapScroll;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform nodeContainer;
    [SerializeField] public RectTransform lineContainer;
    [SerializeField] private GameObject nodePrefab;

    [SerializeField] private MapGenerator generator;

    [Header("Events")]
    [SerializeField] private MapEventDatabase eventDb;
    [SerializeField] private MapEventPanel eventPanel;

    Dictionary<int, MapNodeUI> nodeUIById = new();
    private MapNodeUI currentNode;

    public event System.Action<MapNodeUI> OnNodeSelected;

    private void Awake()
    {
        instance = this;

        var hlg = nodeContainer.GetComponent<HorizontalLayoutGroup>();

        if(!hlg)
        {
            hlg = nodeContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 60f;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = false;
        }

        var csf = nodeContainer.GetComponent<ContentSizeFitter>();
        if(!csf)
        {
            csf = nodeContainer.GetComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
    private void Start()
    {
        if (MapRunData.currentMap == null)
        {
            generator.seed = MapRunData.currentSeed;
            MapRunData.currentMap = generator.GenerateMap();
        }

        PrintMapDebug(MapRunData.currentMap);

        
        StartCoroutine(GenerateMapUIAsync(MapRunData.currentMap));
    }

    private IEnumerator GenerateMapUIAsync(MapData map)
    {
        nodeUIById.Clear();

        
        foreach (Transform child in nodeContainer)
        {
            Destroy(child.gameObject);
        }

        
        for (int col = 0; col < map.columns.Count; col++)
        {
            GameObject columnGroup = new GameObject($"Column{col}", typeof(RectTransform));
            columnGroup.transform.SetParent(nodeContainer);
            columnGroup.transform.localScale = Vector3.one;

            VerticalLayoutGroup layout = columnGroup.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 30;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var le = columnGroup.AddComponent<LayoutElement>();
            le.preferredWidth = 160f;
            le.preferredHeight = 1000f;

            foreach (var node in map.columns[col])
            {
                GameObject nodeGO = Instantiate(nodePrefab, columnGroup.transform);
                MapNodeUI nodeUI = nodeGO.GetComponent<MapNodeUI>();
                nodeUI.Setup(node, this, eventDb, eventPanel);
                nodeUIById[node.id] = nodeUI;
            }
        }

        SetupContainer();

        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(nodeContainer);
        Canvas.ForceUpdateCanvases();

        float padding = 200f;
        float mapWidth = nodeContainer.rect.width + padding;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapWidth);

        lineContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapWidth);

        foreach (Transform ch in lineContainer) Destroy(ch.gameObject);

        foreach (var col in map.columns)
        {
            foreach (var nodeData in col)
            {
                if (nodeUIById.TryGetValue(nodeData.id, out MapNodeUI fromNode))
                {
                    foreach (int targetId in nodeData.connectedTo)
                    {
                        if (nodeUIById.TryGetValue(targetId, out MapNodeUI toNode))
                        {
                            fromNode.AddConnection(toNode);
                            fromNode.DrawConnectionTo(toNode);
                        }
                    }
                }
            }
        }

        
        if (MapRunData.currentNode != null && nodeUIById.TryGetValue(MapRunData.currentNode.id, out var selectedNode))
        {
            SelectNode(selectedNode);
            FocusOnNode(selectedNode, instant: true);
        }
        else
        {
            foreach (var kvp in nodeUIById)
            {
                bool isStart = kvp.Value.GetNodeData().type == NodeType.Start;
                kvp.Value.SetInteractable(isStart);
            }

            MapRunData.currentNode = null;

            var start = nodeUIById.Values.FirstOrDefault(n => n.GetNodeData().type == NodeType.Start);
            if (start != null) FocusOnNode(start, instant: true);
        }

        

        
    }

    private void PrintMapDebug(MapData map)
    {
        Debug.Log($"== MAPA | Seed: {map.seed}");

        for (int col = 0; col < map.columns.Count; col++)
        {
            Debug.Log($"-- Kolumna {col} --");

            foreach (var node in map.columns[col])
            {
                string connections = string.Join(", ", node.connectedTo);
                Debug.Log($"Node {node.id}: {node.type} | FightIndex: {node.fightIndex} | -> {connections}");
            }
        }

        Debug.Log("== KONIEC MAPY ==");
    }

    public void SelectNode(MapNodeUI node)
    {
        OnNodeSelected?.Invoke(node);

        MapRunData.currentNode = node.data;
        currentNode = node;

        foreach (var ui in nodeUIById.Values)
        {
            ui.SetInteractable(false);
        }

        foreach (int id in MapRunData.currentNode.connectedTo)
        {
            if (nodeUIById.TryGetValue(id, out var connectedUI))
            {
                connectedUI.SetInteractable(true);
            }
        }

        MapRunData.currentNode.wasVisisted = true;
        node.UpdateVisual();
    }


    private void SetupContainer()
    {
        content.anchorMin = new Vector2(0, 0);
        content.anchorMax = new Vector2(0, 1);
        content.pivot = new Vector2(0, 0.5f);
        content.anchoredPosition = Vector2.zero;

        nodeContainer.anchorMin = new Vector2(0, 0);
        nodeContainer.anchorMax = new Vector2(0, 1);
        nodeContainer.pivot = new Vector2(0, 0.5f);
        nodeContainer.anchoredPosition = Vector2.zero;

        lineContainer.anchorMin = new Vector2(0, 0);
        lineContainer.anchorMax = new Vector2(0, 1);
        lineContainer.pivot = new Vector2(0, 0.5f);
        lineContainer.anchoredPosition = Vector2.zero;
    }

    public void FocusOnNode(MapNodeUI node, bool instant = true, float lerpTime = 0.25f)
    {
        if (node == null || mapScroll == null || content == null) return;

        float contentW = content.rect.width;
        float viewportW = mapScroll.viewport.rect.width;

        if (contentW <= viewportW) return;

        RectTransform rt = node.GetComponent<RectTransform>();
        Vector3 nodeLocal = content.InverseTransformPoint(rt.TransformPoint(rt.rect.center));

        float nodeX = nodeLocal.x;

        float targetLeft = nodeX - viewportW * 0.5f;
        float maxLeft = contentW - viewportW;
        float normalllized = Mathf.Clamp01(targetLeft / Mathf.Max(1f, maxLeft));

        if(instant)
        {
            mapScroll.horizontalNormalizedPosition = normalllized;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(LerpHorizontal(normalllized, lerpTime));
        }
    }

    private System.Collections.IEnumerator LerpHorizontal(float target, float time)
    {
        float start = mapScroll.horizontalNormalizedPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, time);
            mapScroll.horizontalNormalizedPosition = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        mapScroll.horizontalNormalizedPosition = target;
    }

    public IEnumerable<MapNodeUI> GetAllNodeUI() => nodeUIById.Values;

    public MapNodeUI GetCurrentNodeUI() => currentNode;

    public MapNodeUI GetStartNodeUI() => nodeUIById.Values.FirstOrDefault(n => n.GetNodeData().type == NodeType.Start);
    
}
