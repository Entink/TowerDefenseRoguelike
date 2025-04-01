using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSelectionButton : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;

    private GameObject unitPrefab;
    private UnitSelectionManager selectionManager;

    private bool isSelected = false;

    public void Setup(GameObject prefab, UnitSelectionManager manager)
    {
        unitPrefab = prefab;
        selectionManager = manager;

        label.text = prefab.name;
        GetComponent<Button>().onClick.AddListener(OnClick);
        UpdateVisual();
    }

    private void OnClick()
    {
        isSelected = selectionManager.ToggleSelection(unitPrefab);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        background.color = isSelected ? Color.green : Color.white;
    }


}
