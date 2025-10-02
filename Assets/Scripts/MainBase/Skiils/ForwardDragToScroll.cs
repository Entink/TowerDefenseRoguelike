using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForwardDragToScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect scroll;

    private void Awake()
    {
        if (scroll == null)
            scroll = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (scroll != null) scroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scroll != null) scroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scroll != null) scroll.OnEndDrag(eventData);
    }

}