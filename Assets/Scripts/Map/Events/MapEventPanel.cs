using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MapEventPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image art;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private MapEventOptionButton optionPrefab;
    [SerializeField] private Button closeButton;

    private Action onResolved;
    private MapEventDef current;

    private void Awake()
    {
        if (closeButton) closeButton.onClick.AddListener(() => { Close(); onResolved?.Invoke(); });
        gameObject.SetActive(false);
    }

    public void Show(MapEventDef def, Action onResolved)
    {
        current = def;
        this.onResolved = onResolved;

        if (art) art.sprite = def.illustration;
        if (titleText) titleText.text = def.title;
        if (bodyText) bodyText.text = def.body;

        foreach (Transform ch in optionsContainer) Destroy(ch.gameObject);

        foreach(var opt in def.options)
        {
            var ui = Instantiate(optionPrefab, optionsContainer);
            ui.Setup(opt, () => { Apply(opt); Close(); onResolved?.Invoke(); });
        }

        gameObject.SetActive(true);
    }

    void Apply(EventOption opt)
    {
        if (opt.deltaCash != 0) RunResources.AddCash(opt.deltaCash);
        if (opt.deltaMaterials != 0) RunResources.AddMaterials(opt.deltaMaterials);

        if(opt.addModifier)
        {
            RunData.I.AddModifier(opt.modifier, opt.modifierStacks);
        }
    }

    public void Close() => gameObject.SetActive(false);
}