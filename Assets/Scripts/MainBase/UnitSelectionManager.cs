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
    public int maxSelectableUnits = 5;

    private List<GameObject> selectedUnits = new List<GameObject>();

    private void Start()
    {
        
    }

    private void GenerateUnitButtons()
    {
        foreach(var unit in allUnits)
        {
            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            UnitSelectionButton usb = buttonObj.GetComponent<UnitSelectionButton>();
            //usb.Setup(unit, this);
        }
    }

    public void ToggleSelection(GameObject unit)
    {
        if(selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
        }
        else
        {
            if(selectedUnits.Count >= maxSelectableUnits)
            {
                return;
            }
            selectedUnits.Add(unit);
        }

        confirmButton.interactable = selectedUnits.Count > 0;

    }

    private void OnConfirm()
    {
        RunData.selectedUnits = selectedUnits;
        SceneLoader.LoadScene("MapScene");
    }
}
