using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitSelectionManager : MonoBehaviour
{
    [Header("Dostepne jednostki")]
    public List<GameObject> allUnits;

    [Header("UI")]
    public Transform unitButtonContainer;
    public GameObject unitButtonPrefab;
    public Button confirmButton;

    [Header("Settings")]
    public int maxSelectableUnits = 3;

    private List<GameObject> selectedUnits = new List<GameObject>();

    private void Awake()
    {
        maxSelectableUnits = UpgradeManager.GetCurrentUnitLimit();

    }

    private void Start()
    {
        GenerateUnitButtons();
        confirmButton.interactable = false;
        confirmButton.onClick.AddListener(OnConfirm);
    }

    private void GenerateUnitButtons()
    {
        foreach(var unit in allUnits)
        {
            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            UnitSelectionButton usb = buttonObj.GetComponent<UnitSelectionButton>();
            usb.Setup(unit, this);
        }
    }

    public bool ToggleSelection(GameObject unit)
    {
        if(selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            confirmButton.interactable = selectedUnits.Count > 0;
            return false;
        }
        else
        {
            if(selectedUnits.Count >= maxSelectableUnits)
            {
                return false;
            }
            selectedUnits.Add(unit);
            confirmButton.interactable = true;
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
