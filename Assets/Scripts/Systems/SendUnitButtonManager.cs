using UnityEngine;
using System.Collections.Generic;

public class SendUnitButtonManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public UnitSpawner spawner;

    private void Awake()
    {
        List<GameObject> selectedUnits = RunData.selectedUnits;

        if(selectedUnits == null || selectedUnits.Count == 0)
        {
            Debug.LogWarning("Brak wybranych jednostek.");
            return;
        }

        spawner.units = new List<UnitTemplate>();

        for(int i = 0; i < selectedUnits.Count; i++)
        {
            GameObject unit = selectedUnits[i];
            KeyCode hotkey = GetHotkey(i);

            UnitTemplate template = new UnitTemplate(unit, hotkey);
            spawner.units.Add(template);

            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);
            UnitButtonUI buttonUI = buttonGO.GetComponent<UnitButtonUI>();

            

            buttonUI.unitIndex = i;
            buttonUI.spawner = spawner;

        }

        Debug.Log($"Przypisano {spawner.units.Count} jednostek do spawnera.");
    }

    private KeyCode GetHotkey(int index)
    {
        if (index >= 0 && index <= 8)
        {
            return KeyCode.Alpha1 + index;
        }
        else if (index == 9)
        {
            return KeyCode.Alpha0;
        }
        else
        {
            return KeyCode.None;
        }
    }
}
