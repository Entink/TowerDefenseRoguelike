using UnityEngine;
using System.Collections.Generic;

public class ModifierChoicePanel : MonoBehaviour
{
    [SerializeField] private ModifierOptionUI optionPrefab;
    [SerializeField] private Transform container;

    public void ShowOptions(List<RunModifierDef> defs, System.Action<RunModifierDef> onChoose)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach(var def in defs)
        {
            var ui = Instantiate(optionPrefab, container);
            ui.SetUp(def, (choosen) =>
            {
                onChoose?.Invoke(choosen);
                gameObject.SetActive(false);
            });
        }
        gameObject.SetActive(true);
    }
}