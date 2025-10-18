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

    [Header("Progression")]
    [SerializeField] private ProgressConfig progression;

    Dictionary<int, MapNodeUI> nodeUIById = new();
    private MapNodeUI currentNode;

    public event System.Action<MapNodeUI> OnNodeSelected;
    public static System.Action OnMapReady;

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

        if (progression == null) progression = Resources.Load<ProgressConfig>("ProgressionConfig");
        AssignFightsByDifficulty(MapRunData.currentMap);

        if(TutorialState.I != null && TutorialState.I.Active && MapRunData.currentMap.columns.Count > 1)
        {
            bool includeTutorial = true;
            var listInclTut = FightDatabase.instance.GetNonBossForAct(MapRunData.currentAct, includeTutorial);

            var tut = FightDatabase.instance.allFights.FirstOrDefault(f => f != null && f.isTutorialOnly);
            if(tut != null)
            {
                int tutIdx = listInclTut.FindIndex(f => f == tut);
                if (tutIdx < 0) tutIdx = 0;

                foreach (var node in MapRunData.currentMap.columns[1])
                {
                    node.type = NodeType.Fight;
                    node.fightIndex = tutIdx;
                }
            }
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
            bool alreadyVisited = MapRunData.currentNode.wasVisisted;
            SelectNode(selectedNode, markVisited: alreadyVisited);
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

        if(MapRunData.nodeToMarkVisited != -1 &&
            nodeUIById.TryGetValue(MapRunData.nodeToMarkVisited, out var toVisit))
        {
            SelectNode(toVisit, markVisited: true, updateInteractable: true);
            MapRunData.nodeToMarkVisited = -1;
        }


        OnMapReady?.Invoke();
        OnMapReady = null;
        

        
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

    public void SelectNode(MapNodeUI node, bool markVisited = true, bool updateInteractable = true)
    {
        OnNodeSelected?.Invoke(node);

        MapRunData.currentNode = node.data;
        currentNode = node;

        if(updateInteractable)
        {
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
        }

        if(markVisited)
        {
            MapRunData.currentNode.wasVisisted = true;
            node.UpdateVisual();
        }

        

        
        RunSaveManager.Save();
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

    public static MapNodeData FindNodeById(int id)
    {
        var map = MapRunData.currentMap;
        if (map == null) return null;

        for(int c = 0; c < map.columns.Count; c++)
        {
            var col = map.columns[c];
            for (int i = 0; i < col.Count; i++)
            {
                if (col[i].id == id)
                    return col[i];
            }
        }
        return null;
    }

    private void AssignFightsByDifficulty(MapData map)
    {
        if (map == null) return;
        if (progression == null) return;

        var nonBossList = FightDatabase.instance.GetNonBossForAct(MapRunData.currentAct);
        var bossList = FightDatabase.instance.GetBossesForAct(MapRunData.currentAct);


        int columnsCount = map.columns.Count;
        if (columnsCount <= 2) return;

        for (int col = 0; col < columnsCount; col++)
        {
            foreach(var node in map.columns[col])
            {
                if (node == null) continue;

                if (node.type == NodeType.Boss)
                {
                    if (node.fightIndex < 0 && bossList.Count > 0)
                    {
                        var rng = new System.Random(unchecked(MapRunData.currentSeed ^ (node.id * 313556)));
                        int idx = rng.Next(bossList.Count);
                        node.fightIndex = idx;

                    }
                }
                else if (node.type == NodeType.Fight)
                {
                    int fightCols = columnsCount - 1;
                    float t = Mathf.InverseLerp(1, fightCols - 1, col);
                    t = Mathf.Clamp01(t);

                    float c = progression.curve.Evaluate(t);
                    float targetF = Mathf.Lerp(progression.minDifficulty, progression.maxDifficulty, c) * progression.actDifficultyMul;
                    int target = Mathf.RoundToInt(targetF);

                    var rng = new System.Random(unchecked(MapRunData.currentSeed ^ (node.id * 542346)));
                    int roll = target + rng.Next(-progression.variance, progression.variance + 1);
                    roll = Mathf.Clamp(roll, progression.minDifficulty, progression.maxDifficulty);

                    int chosenIndexInNonBoss = PickIndexClosestByDifficulty(nonBossList, roll, rng);
                    if (chosenIndexInNonBoss >= 0)
                    {
                        node.fightIndex = chosenIndexInNonBoss;
                    }
                }
                
            }
        }

        Debug.Log($"[MapManager] Assigned fights for Act {MapRunData.currentAct} ({nonBossList.Count} fights, {bossList.Count} bosses)");
    }

    private int PickIndexClosestByDifficulty(List<FightData> list, int targetDiff, System.Random rng)
    {
        if (list == null || list.Count == 0) return -1;
        int bestIdx = -1;
        int bestScore = int.MaxValue;

        for(int i = 0; i < list.Count; i++)
        {
            var f = list[i];
            if (f == null) continue;

            int score = Mathf.Abs(f.difficulty - targetDiff);
            if(score < bestScore)
            {
                bestScore = score;
                bestIdx = i;
            }
            else if (score == bestScore)
            {
                if (rng.NextDouble() < 0.5) bestIdx = i;
            }
        }
        return bestIdx;
    }
    
}
