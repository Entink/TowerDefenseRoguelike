using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    

    [Header("UI")]
    [SerializeField] private Transform nodeContainer;
    [SerializeField] private GameObject nodePrefab;
    public Transform lineContainer;

    [SerializeField] private MapGenerator generator;

    Dictionary<int, MapNodeUI> nodeUIById = new();
    private MapNodeUI currentNode;


    private void Awake()
    {
        instance = this;
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

            foreach (var node in map.columns[col])
            {
                GameObject nodeGO = Instantiate(nodePrefab, columnGroup.transform);
                MapNodeUI nodeUI = nodeGO.GetComponent<MapNodeUI>();
                nodeUI.Setup(node, this);
                nodeUIById[node.id] = nodeUI;
            }
        }

        
        yield return null;
        Canvas.ForceUpdateCanvases();

        
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
        }
        else
        {
            foreach (var kvp in nodeUIById)
            {
                bool isStart = kvp.Value.GetNodeData().type == NodeType.Start;
                kvp.Value.SetInteractable(isStart);
            }

            MapRunData.currentNode = null;
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

    
}
