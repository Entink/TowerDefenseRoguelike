using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UnitSelectionButton : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Locked Overlay")]
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button unlockButton;

    [Header("Skill Tree (MainBase)")]
    [SerializeField] public UnitSkillTreePanel skillTreePanel;

    private UnitDef def;
    private GameObject unitPrefab;
    private UnitSelectionManager selectionManager;

    private bool isSelected = false;
    private bool isUnlocked = false;

    public void Setup(UnitDef def, UnitSelectionManager manager)
    {
        this.def = def;
        this.unitPrefab = def.unitPrefab;
        this.selectionManager = manager;

        label.text = def.displayName;

        isUnlocked = UnitUnlocks.IsUnlocked(def.id);


        if (lockedOverlay) lockedOverlay.SetActive(!isUnlocked);
        if (costText) costText.text = (def.unlockMethod == UnitUnlockMethod.Materials)
                ? $"{def.costMaterials} materials"
                : "Unlock during run";
        
        if(unlockButton)
        {
            unlockButton.onClick.RemoveAllListeners();

            bool showUnlockBtn = (def.unlockMethod == UnitUnlockMethod.Materials);
            unlockButton.gameObject.SetActive(showUnlockBtn);

            if(showUnlockBtn)
            {
                unlockButton.onClick.AddListener(() =>
                {
                    if (UnitUnlocks.TryUnlockWithMaterials(def))
                    {
                        isUnlocked = true;
                        if (lockedOverlay) lockedOverlay.SetActive(false);
                        UpdateVisual();
                        selectionManager.RefreshConfirmButton();

                    }
                    else
                    {
                        Debug.Log("Cannot unlock unit: missing requirements or not enough materials.");
                    }
                });
            }

            
        }

        var btn = GetComponent<Button>();
        if(btn!= null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnLeftClick);
        }

        UpdateVisual();
    }

    private void OnLeftClick()
    {
        if (!isUnlocked) return;

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

        if(def == null)
        {
            Debug.LogWarning("UnitSelectionButton: def is null");
            return;
        }

        
        skillTreePanel.gameObject.SetActive(true);
        skillTreePanel.Open(def.id);
    }

    private void UpdateVisual()
    {
        if(background)
            background.color = isSelected ? new Color(0.6f, 1f, 0.6f, 1f) : Color.white;

        if (!isUnlocked && background)
            background.color = new Color(1f, 1f, 1f, 0.5f);

        if (isUnlocked)
        {
            unlockButton.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
        }
    }


}
