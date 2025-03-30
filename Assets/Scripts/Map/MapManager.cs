using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform nodeContainer;
    [SerializeField] private GameObject nodePrefab;


    [SerializeField] private MapGenerator generator;
    


    private void Start()
    {
        MapRunData.currentSeed = generator.seed;
        MapRunData.currentMap = generator.GenerateMap();

        PrintMapDebug(MapRunData.currentMap);
        GenerateMapUI(MapRunData.currentMap);
    }

    private void GenerateMapUI(MapData map)
    {
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
                nodeUI.Setup(node);
            }
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
}
