using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger_Building : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipText;
    RectTransform rt;

    private void Awake()
    {
        rt = transform as RectTransform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null || rt == null) return;
        ModifierTooltip.I.ShowAt(rt,tooltipText,ModifierTooltip.TooltipSide.Auto);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.Hide();
    }
}
