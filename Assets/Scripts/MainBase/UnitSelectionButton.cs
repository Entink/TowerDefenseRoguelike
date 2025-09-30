using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UnitSelectionButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;

    private GameObject unitPrefab;
    private UnitSelectionManager selectionManager;

    [Header("Skill Tree (MainBase)")]
    [SerializeField] public UnitSkillTreePanel skillTreePanel;

    private bool isSelected = false;

    public void Setup(GameObject prefab, UnitSelectionManager manager)
    {
        unitPrefab = prefab;
        selectionManager = manager;

        label.text = prefab.name;
        GetComponent<Button>().onClick.AddListener(OnLeftClick);
        UpdateVisual();
    }

    private void OnLeftClick()
    {
        isSelected = selectionManager.ToggleSelection(unitPrefab);
        UpdateVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;

        if (skillTreePanel == null)
        {
            Debug.LogWarning("UnitSkillTreePanel reference not set on UnitSelectionButton.");
            return;
        }

        var stats = unitPrefab.GetComponent<UnitStats>();
        if (stats == null) { Debug.LogWarning("UnitStats missing on prefab."); return; }

        
        skillTreePanel.gameObject.SetActive(true);
        skillTreePanel.Open(stats.unitId);
    }

    private void UpdateVisual()
    {
        background.color = isSelected ? Color.green : Color.white;
    }


}
