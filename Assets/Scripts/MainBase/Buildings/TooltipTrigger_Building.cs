using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger_Building : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.Show(tooltipText);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.Hide();
    }
}
