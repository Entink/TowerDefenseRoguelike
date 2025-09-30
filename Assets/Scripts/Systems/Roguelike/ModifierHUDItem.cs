using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ModifierHUDItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI stackText;

    RunModifierDef _def;
    int _stacks;

    public void Setup(RunModifierDef def, int stacks)
    {
        _def = def;
        _stacks = stacks;

        if (icon != null && def.icon != null)
            icon.sprite = def.icon;

        if (stackText != null)
            stackText.text = stacks > 1 ? $"x{stacks}" : "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null) return;
        string text = RunModifierFormatting.BuildTooltip(_def, _stacks);
        ModifierTooltip.I.Show(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.Hide();
    }
}