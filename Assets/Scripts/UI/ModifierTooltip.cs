using UnityEngine;
using TMPro;

public class ModifierTooltip : MonoBehaviour
{
    public static ModifierTooltip I { get; private set; }

    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI body;
    [SerializeField] Vector2 offset = new Vector2(16f, 16f);

    Canvas _canvas;
    Camera _uiCamera;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        _canvas = GetComponentInParent<Canvas>();
        _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;

        Hide();
    }

    private void Update()
    {
        if (!panel.gameObject.activeSelf) return;
        Vector2 pos = Input.mousePosition;
        if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay && _uiCamera != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, pos, _uiCamera, out var local);
            panel.anchoredPosition = local + offset;
        }
        else
        {
            panel.anchoredPosition = (Vector2)pos + offset;
        }
    }

    public void Show(string text)
    {
        if (body != null) body.text = text;
        panel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}