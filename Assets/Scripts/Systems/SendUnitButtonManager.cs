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

        spawner.units = new UnitSpawner.SpawnableUnit[selectedUnits.Count];

        for(int i = 0; i < selectedUnits.Count; i++)
        {
            GameObject unit = selectedUnits[i];

            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);
            UnitButtonUI buttonUI = buttonGO.GetComponent<UnitButtonUI>();

            

            buttonUI.unitIndex = i;
            buttonUI.spawner = spawner;

            spawner.units[i] = new UnitSpawner.SpawnableUnit
            {
                prefab = unit,
                hotkey = KeyCode.Alpha1 + i
            };
        }

        Debug.Log($"Przypisano {spawner.units.Length} jednostek do spawnera.");
    }
}
