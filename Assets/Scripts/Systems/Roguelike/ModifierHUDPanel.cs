using UnityEngine;

public class ModifierHUDPanel : MonoBehaviour
{
    [SerializeField] private ModifierHUDItem itemPrefab;
    [SerializeField] private Transform container;

    private void OnEnable()
    {
        if (RunData.I != null)
            RunData.I.OnModifiersChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (RunData.I != null)
            RunData.I.OnModifiersChanged -= Refresh;
    }

    void Refresh()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        if (RunData.I == null || RunData.I.activeModifiers == null) return;

        foreach(var state in RunData.I.activeModifiers)
        {
            var def = RunModifierLookup.Def(state.id);
            if (def == null) continue;

            var ui = Instantiate(itemPrefab, container);
            ui.Setup(def, state.stacks);
        }
    }
}