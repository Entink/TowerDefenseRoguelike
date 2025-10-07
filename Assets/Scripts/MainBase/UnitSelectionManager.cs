using UnityEngine;
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


    private void Update()
    {
        maxSelectableUnits = UpgradeManager.GetCurrentUnitLimit();

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
        foreach (Transform ch in unitButtonContainer)
            Destroy(ch.gameObject);

        var skillTree = FindObjectOfType<UnitSkillTreePanel>(true);

        foreach(var def in unitDb.All)
        {
            if (def == null || def.unitPrefab == null) continue;

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
        MapRunData.currentSeed = Random.Range(0, int.MaxValue);
        SceneLoader.LoadScene("MapScene");
    }
}
