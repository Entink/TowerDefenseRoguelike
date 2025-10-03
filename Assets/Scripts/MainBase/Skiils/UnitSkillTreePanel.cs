using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UnitSkillTreePanel : MonoBehaviour
{
    [SerializeField] private UnitSkillTreeDatabase db;
    [SerializeField] private SkillNodeButton nodePrefab;
    [SerializeField] private TextMeshProUGUI header;

    [Header("Graph")]
    [SerializeField] private RectTransform nodeContainer;
    [SerializeField] private RectTransform edgeContainer;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform graphArea;
    [SerializeField] private Vector2 cellSize = new Vector2(180, 120);
    [SerializeField] private Vector2 padding = new Vector2(80, 80);
    [SerializeField] private SkillEdgeUI edgePrefab;
    [SerializeField] private Color edgeLocked = new Color(1, 1, 1, 0.25f);
    [SerializeField] private Color edgeOpen = new Color(1, 1, 1, 0.6f);
    [SerializeField] private Color edgeOwned = new Color(0.2f, 1f, 0.4f, 0.8f);

    [Header("Close controls")]
    [SerializeField] private Button closeButton;

    UnitId currentUnit;
    UnitSkillTreeDef def;

    private void Awake()
    {
        if(closeButton!=null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Input.GetKeyDown(KeyCode.Escape)) Close();
    }

    public void Open(UnitId unit)
    {
        currentUnit = unit;
        def = db != null ? db.Get(unit) : Resources.Load<UnitSkillTreeDatabase>("UnitSkillTrees")?.Get(unit);
        gameObject.SetActive(true);
        Rebuild();

        
    }

    public void Close() => gameObject.SetActive(false);

    void Rebuild()
    {
        foreach (Transform ch in edgeContainer) Destroy(ch.gameObject);
        foreach (Transform ch in nodeContainer) Destroy(ch.gameObject);

        if (header != null) header.text = currentUnit.ToString();
        if (def == null || def.nodes == null || def.nodes.Length == 0) return;

        int minCol = int.MaxValue;
        int maxCol = int.MinValue;
        int minRow = int.MaxValue;
        int maxRow = int.MinValue;

        foreach (var n in def.nodes)
        {
            if (n == null) continue;
            minCol = Mathf.Min(minCol, n.col);
            maxCol = Mathf.Max(maxCol, n.col);
            minRow = Mathf.Min(minRow, n.row);
            maxRow = Mathf.Max(maxRow, n.row);
        }

        int cols = (maxCol - minCol + 1);
        int rows = (maxRow - minRow + 1);

        

        float contentW = cols * cellSize.x + padding.x;
        float contentH = rows * cellSize.y + padding.y;

        graphArea.anchorMin = graphArea.anchorMax = new Vector2(0, 1);
        graphArea.pivot = new Vector2(0, 1);
        graphArea.sizeDelta = new Vector2(Mathf.Max(contentW,1f), Mathf.Max(contentH,1f));

        var map = new Dictionary<string, SkillNodeButton>();
        foreach(var node in def.nodes)
        {
            if (node == null) continue;

            int level = UnitSkillProgress.GetLevel(currentUnit, node.nodeId);
            var ui = Instantiate(nodePrefab, nodeContainer);
            ui.Setup(currentUnit, def, node, level, OnBuy);
            map[node.nodeId] = ui;

            var rt = (RectTransform)ui.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);

            float x = padding.x + (node.col - minCol) * cellSize.x;
            float y = -(padding.y + (node.row - minRow) * cellSize.y);

            rt.anchoredPosition = new Vector2(x, y);

            bool prereqOk = ArePrereqsMet(node);
            ui.SetInteractable(prereqOk && level < node.maxLevel);
            ui.SetDim(!prereqOk && level <= 0);
        }

        foreach (var node in def.nodes)
        {
            if (node == null || node.prerequisites == null) continue;
            var toBtn = map[node.nodeId];
            var toRT = (RectTransform)toBtn.transform;

            foreach (var preId in node.prerequisites)
            {
                if (!map.TryGetValue(preId, out var fromBtn)) continue;
                var fromRT = (RectTransform)fromBtn.transform;

                int preLvl = UnitSkillProgress.GetLevel(currentUnit, preId);
                int toLvl = UnitSkillProgress.GetLevel(currentUnit, node.nodeId);

                Color col = edgeLocked;
                if (preLvl > 0 && toLvl > 0) col = edgeOwned;
                else if (preLvl > 0) col = edgeOpen;

                var edge = Instantiate(edgePrefab, edgeContainer);
                edge.Bind(fromRT, toRT, col);
            }
        }
        Canvas.ForceUpdateCanvases();

        Bounds bNodes = RectTransformUtility.CalculateRelativeRectTransformBounds(graphArea, nodeContainer);
        Bounds bEdges = RectTransformUtility.CalculateRelativeRectTransformBounds(graphArea, edgeContainer);

        Bounds b = bNodes;
        b.Encapsulate(bEdges);

        Vector2 contentInnerSize = new Vector2(b.size.x, b.size.y);

        Vector2 contentSize = contentInnerSize + 2f * padding;
        graphArea.sizeDelta = new Vector2(Mathf.Max(1f, contentSize.x), Mathf.Max(1f, contentSize.y));

        Vector2 topLeftOfBounds = new Vector2(b.min.x, b.max.y);

        Vector2 shift = new Vector2(padding.x - topLeftOfBounds.x, -padding.y - topLeftOfBounds.y);

        nodeContainer.anchoredPosition += shift;
        edgeContainer.anchoredPosition += shift;

        CenterAndClampContent();

    }

    void OnBuy(UnitId unit, SkillNode node)
    {
        bool ok = UnitSkillProgress.TryBuy(unit, node, () => RunResources.GetMaterials(), (d) => RunResources.AddMaterials(d));
        Rebuild();
    }

    bool ArePrereqsMet(SkillNode node)
    {
        if (node.prerequisites == null || node.prerequisites.Length == 0) return true;
        foreach (var pre in node.prerequisites)
            if (UnitSkillProgress.GetLevel(currentUnit, pre) < 1) return false;
        return true;
    }

    void CenterAndClampContent()
    {
        if (viewport == null || graphArea == null) return;

        Vector2 view = viewport.rect.size;
        Vector2 cont = graphArea.rect.size;

        float minX = Mathf.Min(0f, view.x - cont.x);
        float maxX = 0f;
        float minY = Mathf.Min(0f, view.y - cont.y);
        float maxY = 0f;

        float centerX = (view.x - cont.x) * 0.5f;
        float centerY = (view.y - cont.y) * 0.5f;

        Vector2 pos = new Vector2(
            Mathf.Clamp(centerX, minX, maxX),
            Mathf.Clamp(centerY, minY, maxY)
            );

        graphArea.anchoredPosition = pos;
    }



    void LateUpdate()
    {
        if (!viewport || !graphArea) return;

        Vector2 view = viewport.rect.size;
        Vector2 cont = graphArea.rect.size;

        float minX = Mathf.Min(0f, view.x - cont.x);
        float maxX = 0f;
        float minY = Mathf.Min(0f, view.y - cont.y);
        float maxY = 0f;

        var p = graphArea.anchoredPosition;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        graphArea.anchoredPosition = p;
    }

}