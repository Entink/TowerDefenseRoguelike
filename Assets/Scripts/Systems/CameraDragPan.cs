using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class CameraDragPan : MonoBehaviour
{
    [Header("Ref")]
    public Camera cam;

    [Header("Pan")]
    [Tooltip("How much pixels per 1 world size unit")]
    public float pixelsPerUnit = 200f;
    public float sensitivity = 1.0f;

    [Header("Clamp (world x)")]
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Blockages")]
    public bool blockWhenOverUI = true;
    public KeyCode holdToPan = KeyCode.Mouse0;

    bool dragging;
    Vector3 lastMouse;

    private void Reset()
    {
        cam = GetComponent<Camera>();
    }

    private void Awake()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(holdToPan))
        {
            if (blockWhenOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            dragging = true;
            lastMouse = Input.mousePosition;
        }
        else if(Input.GetKeyUp(holdToPan))
        {
            dragging = false;
        }

        if (!dragging) return;

        Vector3 cur = Input.mousePosition;
        Vector3 deltaPx = cur - lastMouse;
        lastMouse = cur;

        float dxWorld = -(deltaPx.x / Mathf.Max(1f, pixelsPerUnit)) * sensitivity;

        var p = cam.transform.position;
        p.x = Mathf.Clamp(p.x + dxWorld, minX, maxX);
        cam.transform.position = p;
    }

    public void SetClamp(float newMinX, float newMaxX)
    {
        minX = newMinX;
        maxX = newMaxX;

        var p = cam.transform.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        cam.transform.position = p;
    }
}