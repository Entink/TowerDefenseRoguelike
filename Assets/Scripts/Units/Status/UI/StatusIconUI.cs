using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


[RequireComponent(typeof(RectTransform))]
public class StatusIconUI : MonoBehaviour
{
    public StatusIconLibrary library;
    public float refreshInterval = 0.25f;
    public Vector2 iconSize = new Vector2(20, 20);
    public float spacing = 4f;
    public float fontSize = 16f;

    StatusController sc;
    RectTransform rt;
    float timer;

    readonly List<GameObject> pool = new();

    private void Awake()
    {
        sc = GetComponentInParent<StatusController>();
        rt = GetComponent<RectTransform>();
        if (sc != null) sc.EffectsChanged += ForceRefresh;
    }

    private void OnEnable()
    {
        ForceRefresh();
    }

    private void OnDisable()
    {
        ClearIcons();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= refreshInterval)
        {
            timer = 0f;
            Refresh();
        }
    }

    void ForceRefresh()
    {
        timer = 0f;
        Refresh();
    }

    void ClearIcons()
    {
        for (int i = 0; i < pool.Count; i++)
            if (pool[i] != null) pool[i].SetActive(false);
    }

    void Refresh()
    {
        if(sc == null || library == null) { ClearIcons(); return; }

        var items = sc.GetIconData();
        EnsurePoolSize(items.Count);

        float x = 0f;
        for(int i = 0; i < items.Count; i++)
        {
            var data = items[i];
            var go = pool[i];
            go.SetActive(true);

            var img = go.GetComponent<Image>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>(true);

            
            var sp = library.Get(data.typeName);
            if (sp != null) img.sprite = sp;

            var rti = img.rectTransform;
            rti.sizeDelta = iconSize;
            rti.anchoredPosition = new Vector2(x, 0f);
            x += iconSize.x + spacing;

            if(txt != null)
            {
                txt.text = data.stacks > 1 ? data.stacks.ToString() : "";
            }
            
        }

        for (int i = items.Count; i < pool.Count; i++)
            pool[i].SetActive(false);
    }

    void EnsurePoolSize(int n)
    {
        while (pool.Count < n)
        {
            var go = new GameObject("StatusIcon", typeof(RectTransform), typeof(Image));
            var r = go.GetComponent<RectTransform>();
            r.SetParent(rt, false);
            r.anchorMin = new Vector2(0, 0.5f);
            r.anchorMax = new Vector2(0, 0.5f);
            r.pivot = new Vector2(0, 0.5f);

            var txtGo = new GameObject("Stack", typeof(RectTransform), typeof(TextMeshProUGUI));
            var tr = txtGo.GetComponent<RectTransform>();
            tr.SetParent(r, false);
            tr.anchorMin = new Vector2(1, 0);
            tr.anchorMax = new Vector2(1, 0);
            tr.pivot = new Vector2(1, 0);
            tr.anchoredPosition = new Vector2(0, 0);
            var tmp = txtGo.GetComponent<TextMeshProUGUI>();
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.BottomRight;

            pool.Add(go);
            go.SetActive(false);
        }
    }
}
