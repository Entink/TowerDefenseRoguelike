using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitSelectionManager : MonoBehaviour
{
    [Header("Dostepne jednostki")]
    [SerializeField] private UnitDatabase unitDb;

    [Header("UI")]
    public Transform unitButtonContainer;
    public GameObject unitButtonPrefab;
    public Button confirmButton;

    [Header("Settings")]
    public int maxSelectableUnits = 3;

    private List<GameObject> selectedUnits = new List<GameObject>();

    public event System.Action<int> OnSelectedUnitsChanged;


    private void OnEnable()
    {
        UnitUnlocks.Load(unitDb);

        maxSelectableUnits = UpgradeManager.GetCurrentUnitLimit();
        GenerateUnitButtons();

    }



    private void Start()
    {
        UnitUnlocks.Load(unitDb);
        maxSelectableUnits = UpgradeManager.GetCurrentUnitLimit();

        GenerateUnitButtons();
        confirmButton.interactable = false;
        confirmButton.onClick.AddListener(OnConfirm);
    }

    private void GenerateUnitButtons()
    {
        if(!unitButtonContainer || !unitButtonPrefab || !unitDb)
        {
            Debug.LogError("[UnitSelection] No refference UnitButtonContainer/UnitButtonPrefab/UnitDatabase");
            return;
        }

        foreach (Transform ch in unitButtonContainer)
            Destroy(ch.gameObject);

        var skillTree = FindObjectOfType<UnitSkillTreePanel>(true);

        foreach(var def in unitDb.All)
        {
            if (def == null || def.unitPrefab == null) continue;
            if (!UnitUnlocks.IsUnlocked(def.id)) continue;

            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            var usb = buttonObj.GetComponent<UnitSelectionButton>();
            

            if(usb == null)
            {
                Debug.LogError("UnitSelectionButton prefab missing UnitSelectionButton component.");
                continue;
            }

            usb.skillTreePanel = skillTree;

            usb.Setup(def, this);
        }
    }

    public void RefreshConfirmButton()
    {
        confirmButton.interactable = selectedUnits.Count > 0;
        OnSelectedUnitsChanged?.Invoke(selectedUnits.Count);
    }

    public bool ToggleSelection(GameObject unit)
    {
        if(selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            RefreshConfirmButton();
            return false;
        }
        else
        {
            if(selectedUnits.Count >= maxSelectableUnits)
            {
                return false;
            }
            selectedUnits.Add(unit);
            RefreshConfirmButton();
            return true;
        }

        

    }

    private void OnConfirm()
    {
        RunData.selectedUnits = selectedUnits;
        if(TutorialState.I != null && TutorialState.I.Active)
        {
            MapRunData.currentSeed = TutorialState.I.Profile.tutorialSeed;
        }
        else
        {
            MapRunData.currentSeed = Random.Range(0, int.MaxValue);

        }
        SceneLoader.LoadScene("MapScene");
    }
}
