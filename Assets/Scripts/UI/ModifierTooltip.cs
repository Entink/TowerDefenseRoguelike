using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModifierTooltip : MonoBehaviour
{
    public static ModifierTooltip I { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI body;
    [SerializeField] private LayoutElement layoutElement;

    [Header("Sizing")]
    [SerializeField] private float fixedWidth = 260f;
    [SerializeField] private float edgePadding = 8f;

    [Header("Mouse Positioning")]
    [SerializeField] private float cursorVerticalOffset = 16f;

    private Canvas canvas;
    private RectTransform canvasRT;
    private Camera uiCamera;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

        canvas = GetComponentInParent<Canvas>();
        canvasRT = canvas.transform as RectTransform;
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        if (layoutElement == null)
            layoutElement = panel.GetComponent<LayoutElement>();

        if (layoutElement == null)
            layoutElement = panel.gameObject.AddComponent<LayoutElement>();

        layoutElement.preferredWidth = fixedWidth;

        Hide();

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.gameObject.AddComponent<CanvasGroup>();

        cg.blocksRaycasts = false;
        cg.interactable = false;

        DisableRaycastTargets(panel);
    }

    private void Update()
    {
        if (!panel.gameObject.activeSelf)
            return;

        RepositionAtMouse();
    }

    public void Show(string text)
    {
        Debug.Log($"Tooltip Show called with text: {text}");

        if (body != null)
            body.text = text;

        if (layoutElement != null)
            layoutElement.preferredWidth = fixedWidth;

        panel.gameObject.SetActive(true);
        RepositionAtMouse();
    }

    public void ShowAt(RectTransform target, string text, TooltipSide side = TooltipSide.Auto)
    {
        Show(text);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }

    private void RepositionAtMouse()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

        Vector2 mouseLocal = ScreenToCanvasLocal(Input.mousePosition);
        Vector2 size = panel.rect.size;

        Vector2 abovePivot = new Vector2(0.5f, 0f);
        Vector2 belowPivot = new Vector2(0.5f, 1f);

        Vector2 abovePos = mouseLocal + new Vector2(0f, cursorVerticalOffset);
        Vector2 belowPos = mouseLocal + new Vector2(0f, -cursorVerticalOffset);

        bool canFitAbove = FitsInsideCanvas(abovePos, size, abovePivot);
        bool canFitBelow = FitsInsideCanvas(belowPos, size, belowPivot);

        if (canFitAbove)
        {
            panel.pivot = abovePivot;
            panel.anchoredPosition = ClampToCanvas(abovePos, size, abovePivot);
            return;
        }

        if (canFitBelow)
        {
            panel.pivot = belowPivot;
            panel.anchoredPosition = ClampToCanvas(belowPos, size, belowPivot);
            return;
        }

        float canvasCenterY = 0f;

        if (mouseLocal.y >= canvasCenterY)
        {
            panel.pivot = belowPivot;
            panel.anchoredPosition = ClampToCanvas(belowPos, size, belowPivot);
        }
        else
        {
            panel.pivot = abovePivot;
            panel.anchoredPosition = ClampToCanvas(abovePos, size, abovePivot);
        }
    }

    private Vector2 ScreenToCanvasLocal(Vector3 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screenPos, uiCamera, out Vector2 local);
        return local;
    }

    private bool FitsInsideCanvas(Vector2 anchoredPos, Vector2 size, Vector2 pivot)
    {
        Rect c = canvasRT.rect;

        float left = anchoredPos.x - size.x * pivot.x;
        float right = anchoredPos.x + size.x * (1f - pivot.x);
        float bottom = anchoredPos.y - size.y * pivot.y;
        float top = anchoredPos.y + size.y * (1f - pivot.y);

        return left >= c.xMin + edgePadding &&
               right <= c.xMax - edgePadding &&
               bottom >= c.yMin + edgePadding &&
               top <= c.yMax - edgePadding;
    }

    private Vector2 ClampToCanvas(Vector2 anchoredPos, Vector2 size, Vector2 pivot)
    {
        Rect c = canvasRT.rect;

        float minX = c.xMin + size.x * pivot.x + edgePadding;
        float maxX = c.xMax - size.x * (1f - pivot.x) - edgePadding;
        float minY = c.yMin + size.y * pivot.y + edgePadding;
        float maxY = c.yMax - size.y * (1f - pivot.y) - edgePadding;

        anchoredPos.x = Mathf.Clamp(anchoredPos.x, minX, maxX);
        anchoredPos.y = Mathf.Clamp(anchoredPos.y, minY, maxY);

        return anchoredPos;
    }

    private void DisableRaycastTargets(Transform t)
    {
        Graphic[] graphics = t.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
            graphics[i].raycastTarget = false;
    }

    public enum TooltipSide
    {
        Auto,
        Above,
        Below,
        Left,
        Right
    }
}