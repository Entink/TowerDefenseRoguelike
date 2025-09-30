using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSkillTreePanel : MonoBehaviour
{
    [SerializeField] private UnitSkillTreeDatabase db;
    [SerializeField] private Transform container;
    [SerializeField] private SkillNodeButton nodePrefab;
    [SerializeField] private TextMeshProUGUI header;

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
        foreach (Transform ch in container) Destroy(ch.gameObject);
        if (header != null) header.text = currentUnit.ToString();
        if (def == null) return;

        foreach(var node in def.nodes)
        {
            var ui = Instantiate(nodePrefab, container);
            int level = UnitSkillProgress.GetLevel(currentUnit, node.nodeId);
            ui.Setup(currentUnit, node, level, OnBuy);
        }
    }

    void OnBuy(UnitId unit, SkillNode node)
    {
        bool ok = UnitSkillProgress.TryBuy(unit, node, () => RunResources.GetMaterials(), (d) => RunResources.AddMaterials(d));
        Rebuild();
    }


}