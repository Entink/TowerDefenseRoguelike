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
    MapEventDef current;
    

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

        optionsContainer.gameObject.SetActive(true);
        if (closeButton) closeButton.gameObject.SetActive(false);

        foreach(var opt in def.options)
        {
            var ui = Instantiate(optionPrefab, optionsContainer);
            ui.Setup(opt, () => ResolveOption(opt,ui));
        }

        gameObject.SetActive(true);
    }

    void ResolveOption(EventOption opt, MapEventOptionButton clicked)
    {
        if (opt.deltaCash != 0) RunResources.AddCash(opt.deltaCash);
        if (opt.deltaMaterials != 0) RunResources.AddMaterials(opt.deltaMaterials);
        if (opt.addModifier) RunData.I.AddModifier(opt.modifier, opt.modifierStacks);

        if (art && opt.resultIllustration) art.sprite = opt.resultIllustration;
        if (titleText) titleText.text = current.title;
        if (bodyText) bodyText.text = string.IsNullOrWhiteSpace(opt.resultText)
                ? BuildAutoResultText(opt)
                : opt.resultText;


        foreach(Transform t in optionsContainer)
        {
            var b = t.GetComponent<Button>();
            if (b) b.interactable = false;
        }

        optionsContainer.gameObject.SetActive(false);
        if (closeButton) closeButton.gameObject.SetActive(true);
    }

    string BuildAutoResultText(EventOption opt)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder("Outcome:\n");
        if (opt.deltaCash != 0) sb.AppendLine(opt.deltaCash > 0 ? $"+{opt.deltaCash} cash" : $"{opt.deltaCash} cash");
        if (opt.deltaMaterials != 0) sb.AppendLine(opt.deltaMaterials > 0 ? $"+{opt.deltaMaterials} materials" : $"{opt.deltaMaterials} materials");
        if (opt.addModifier) sb.AppendLine($"Gained modifier: {opt.modifier} x{opt.modifierStacks}");
        return sb.ToString();
    }

    public void Close() => gameObject.SetActive(false);
}