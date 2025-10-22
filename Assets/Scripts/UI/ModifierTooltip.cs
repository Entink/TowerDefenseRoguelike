using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModifierTooltip : MonoBehaviour
{
    public static ModifierTooltip I { get; private set; }

    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI body;
    [SerializeField] Vector2 offset = new Vector2(16f, 16f);
    [SerializeField] float edgePadding = 8f;

    public enum TooltipSide { Auto, Above, Below, Left, Right }
    enum AnchorMode { Mouse, Target }

    Canvas _canvas;
    RectTransform _canvasRT;
    Camera _uiCamera;

    AnchorMode _mode = AnchorMode.Mouse;
    RectTransform _target;
    TooltipSide _side = TooltipSide.Auto;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        _canvas = GetComponentInParent<Canvas>();
        _canvasRT = _canvas.transform as RectTransform;
        _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;

        Hide();
        var cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        DisableRaycastTargets(panel);
    }

    void Update()
    {
        if (!panel.gameObject.activeSelf) return;

        if (_mode == AnchorMode.Mouse)
        {
            Vector2 mouseLocal = ScreenToCanvasLocal(Input.mousePosition);
            PlaceNearPoint(mouseLocal, GuessSideForPoint(mouseLocal));
        }
        else
        {
            if (_target == null) { Hide(); return; }
            PlaceNearRect(_target, _side);
        }
    }

    Vector2 ScreenToCanvasLocal(Vector3 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRT, screenPos, _uiCamera, out var local);
        return local;
    }

    Vector2 WorldToCanvasLocal(Vector3 worldPos)
    {
        var screen = RectTransformUtility.WorldToScreenPoint(_uiCamera, worldPos);
        return ScreenToCanvasLocal(screen);
    }

    TooltipSide GuessSideForPoint(Vector2 p)
    {
        var r = _canvasRT.rect;
        float rightSpace = r.xMax - p.x;
        float leftSpace = p.x - r.xMin;
        float upSpace = r.yMax - p.y;
        float downSpace = p.y - r.yMin;

        if (rightSpace >= leftSpace && rightSpace >= upSpace && rightSpace >= downSpace) return TooltipSide.Right;
        if (leftSpace >= upSpace && leftSpace >= downSpace) return TooltipSide.Left;
        if (upSpace >= downSpace) return TooltipSide.Above;
        return TooltipSide.Below;
    }

    void PlaceNearPoint(Vector2 anchorLocal, TooltipSide side)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        Vector2 size = panel.rect.size;
        Rect c = _canvasRT.rect;

        Vector2 dir = side switch
        {
            TooltipSide.Right => new Vector2(1, 0),
            TooltipSide.Left => new Vector2(-1, 0),
            TooltipSide.Above => new Vector2(0, 1),
            TooltipSide.Below => new Vector2(0, -1),
            _ => new Vector2(1, 0)
        };

        Vector2 off = new Vector2(Mathf.Abs(offset.x) * Mathf.Sign(dir.x), Mathf.Abs(offset.y) * Mathf.Sign(dir.y));

        Vector2 pivot = new Vector2(
            dir.x >= 0 ? 0f : 1f,
            dir.y >= 0 ? 0f : 1f
        );
        if (dir.y == 0) pivot.y = 0.5f;
        if (dir.x == 0) pivot.x = 0.5f;
        panel.pivot = pivot;

        Vector2 pos = anchorLocal + off;

        float minX = c.xMin + size.x * panel.pivot.x + edgePadding;
        float maxX = c.xMax - size.x * (1f - panel.pivot.x) - edgePadding;
        float minY = c.yMin + size.y * panel.pivot.y + edgePadding;
        float maxY = c.yMax - size.y * (1f - panel.pivot.y) - edgePadding;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        panel.anchoredPosition = pos;
    }

    void PlaceNearRect(RectTransform target, TooltipSide preferred)
    {
        var r = target.rect;

        Vector2 top = WorldToCanvasLocal(target.TransformPoint(new Vector3(0, r.yMax, 0)));
        Vector2 bottom = WorldToCanvasLocal(target.TransformPoint(new Vector3(0, r.yMin, 0)));
        Vector2 left = WorldToCanvasLocal(target.TransformPoint(new Vector3(r.xMin, 0, 0)));
        Vector2 right = WorldToCanvasLocal(target.TransformPoint(new Vector3(r.xMax, 0, 0)));

        TooltipSide side = preferred == TooltipSide.Auto ? ChooseSideByFreeSpace(top, bottom, left, right) : preferred;

        Vector2 anchor = side switch
        {
            TooltipSide.Above => top,
            TooltipSide.Below => bottom,
            TooltipSide.Left => left,
            TooltipSide.Right => right,
            _ => right
        };

        PlaceNearPoint(anchor, side);
    }

    TooltipSide ChooseSideByFreeSpace(Vector2 top, Vector2 bottom, Vector2 left, Vector2 right)
    {
        Rect c = _canvasRT.rect;
        float freeTop = c.yMax - top.y;
        float freeBottom = bottom.y - c.yMin;
        float freeLeft = left.x - c.xMin;
        float freeRight = c.xMax - right.x;

        if (freeRight >= freeLeft && freeRight >= freeTop && freeRight >= freeBottom) return TooltipSide.Right;
        if (freeLeft >= freeTop && freeLeft >= freeBottom) return TooltipSide.Left;
        if (freeTop >= freeBottom) return TooltipSide.Above;
        return TooltipSide.Below;
    }

    public void Show(string text)
    {
        if (body) body.text = text;
        _mode = AnchorMode.Mouse;
        _target = null;
        _side = TooltipSide.Auto;
        panel.gameObject.SetActive(true);
        Update();
    }

    public void ShowAt(RectTransform target, string text, TooltipSide side = TooltipSide.Auto)
    {
        if (body) body.text = text;
        _mode = AnchorMode.Target;
        _target = target;
        _side = side;
        panel.gameObject.SetActive(true);
        Update();
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
        _target = null;
        _mode = AnchorMode.Mouse;
        _side = TooltipSide.Auto;
    }


    void DisableRaycastTargets(Transform t)
    {
        var graphics = t.GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
        for (int i = 0; i < graphics.Length; i++) graphics[i].raycastTarget = false;
    }
}
