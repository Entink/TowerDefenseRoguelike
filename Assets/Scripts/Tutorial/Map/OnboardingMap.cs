using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class OnboardingMap : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private RectTransform highlightFrame;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private Button skipButton;

    [Header("Map refs")]
    [SerializeField] private MapManager mapManager;
    [SerializeField] private RectTransform canvasRect;

    private int step = 0;
    private MapNodeUI targetNode;
    private List<MapNodeUI> allNodes = new();

    private void Start()
    {
        if(TutorialState.I == null || !TutorialState.I.Active)
        {
            gameObject.SetActive(false);
            return;
        }

        overlayRoot.SetActive(false);
        StartCoroutine(BeginNextFrame());
    }

    private System.Collections.IEnumerator BeginNextFrame()
    {
        yield return null;
        yield return null;

        allNodes = mapManager.GetAllNodeUI().ToList();
        skipButton.onClick.AddListener(SkipAll);

        if(MapRunData.currentNode == null)
        {
            var start = mapManager.GetStartNodeUI();
            if(start != null)
            {
                step = 1;
                overlayRoot.SetActive(true);
                stepText.text = "Click START node, to activate paths.";
                FrameUI(start.GetComponent<RectTransform>());
                SetOnlyTheseNodesInteractable(new[] { start });
                mapManager.OnNodeSelected += OnNodeSelected;
                yield break;
            }
        }

        GoStep2_SelectNextReachable();
    }

    private void GoStep2_SelectNextReachable()
    {
        var current = mapManager.GetCurrentNodeUI();
        if(current == null)
        {
            StartCoroutine(BeginNextFrame());
            return;
        }

        var reachable = current.GetConnectedNodes();
        targetNode = reachable.FirstOrDefault(n => n.GetNodeData().type == NodeType.Fight) ?? reachable.FirstOrDefault();

        if(targetNode == null)
        {
            overlayRoot.SetActive(false);
            gameObject.SetActive(false);
            return;
        }

        step = 2;
        overlayRoot.SetActive(true);
        stepText.text = "Later on you can move map by holding left mouse button and dragging.";

        StartCoroutine(new WaitForSecondsRealtime(5.0f));
        stepText.text = "Click this node to go ahead.";

        FrameUI(targetNode.GetComponent<RectTransform>());

        SetOnlyTheseNodesInteractable(new[] { targetNode });

        mapManager.OnNodeSelected += OnNodeSelected;
    }

    private void OnNodeSelected(MapNodeUI node)
    {
        if(step == 1)
        {
            mapManager.OnNodeSelected -= OnNodeSelected;
            overlayRoot.SetActive(false);
            GoStep2_SelectNextReachable();
            return;
        }

        if(step == 2 && node == targetNode)
        {
            mapManager.OnNodeSelected -= OnNodeSelected;
            RestoreInteractableByMapRules();
            overlayRoot.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void SkipAll()
    {
        mapManager.OnNodeSelected -= OnNodeSelected;
        RestoreInteractableByMapRules();
        overlayRoot.SetActive(false);
        gameObject.SetActive(false);
    }

    private void SetOnlyTheseNodesInteractable(IEnumerable<MapNodeUI> allowed)
    {
        var allowSet = new HashSet<MapNodeUI>(allowed);
        foreach (var n in allNodes)
            n.SetInteractable(allowSet.Contains(n));
    }

    private void RestoreInteractableByMapRules()
    {
        var cur = mapManager.GetCurrentNodeUI();
        if(cur != null)
        {
            mapManager.SelectNode(cur);
        }
        else
        {
            foreach (var ui in allNodes)
                ui.SetInteractable(ui.GetNodeData().type == NodeType.Start);
        }
    }

    private void FrameUI(RectTransform target)
    {
        Vector3[] wc = new Vector3[4];
        target.GetWorldCorners(wc);

        Vector2 min = WorldToCanvas(wc[0]);
        Vector2 max = WorldToCanvas(wc[2]);

        highlightFrame.anchoredPosition = (min + max) / 2f;
        highlightFrame.sizeDelta = (max - min);
    }

    private Vector2 WorldToCanvas(Vector3 world)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, world), null, out var lp);
        return lp;
    }
}
