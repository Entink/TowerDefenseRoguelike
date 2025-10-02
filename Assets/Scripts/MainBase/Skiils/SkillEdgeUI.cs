using UnityEngine;
using UnityEngine.UI;

public class SkillEdgeUI : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private float thickness = 6f;

    RectTransform rt;
    RectTransform a, b;

    private void Awake()
    {
        rt = (RectTransform)transform;
        if (img == null) img = GetComponent<Image>();
        rt.pivot = new Vector2(0f, 0.5f);
    }

    public void Bind(RectTransform from, RectTransform to, Color color)
    {
        a = from; 
        b = to;
        if (img != null) img.color = color;
        UpdateEdge();

    }

    private void LateUpdate()
    {
        if (a == null || b == null) return;
        UpdateEdge();
    }

    void UpdateEdge()
    {
        Vector2 pA = a.anchoredPosition;
        Vector2 pB = b.anchoredPosition;
        Vector2 d = pB - pA;
        float len = d.magnitude;

        rt.anchoredPosition = pA;
        rt.sizeDelta = new Vector2(len, thickness);
        float ang = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, ang);
    }
}