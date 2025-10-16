using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnboardingFight : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private RectTransform highlightFrame;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private Button skipButton;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject background;

    [Header("Refs")]
    [SerializeField] private Transform playerBase;
    [SerializeField] private Transform enemyBase;
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private SendUnitButtonManager buttons;
    [SerializeField] private Transform unitButtonsContainer;
    [SerializeField] private RectTransform goldUI;

    [Header("Camera")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private CameraDragPan dragPan;
    [SerializeField] private CameraControll cameraControll;
    [SerializeField] private float camLerpSpeed = 4f;

    private bool prevDragPanEnabled;
    private bool prevCameraControllEnabled;
    private Vector3 prevCamPos;

    private int step = 0;

    private void Start()
    {
        if (TutorialState.I == null || !TutorialState.I.Active)
        {
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(TimeFreezeWait());

        overlayRoot.SetActive(false);
        skipButton.onClick.AddListener(SkipAll);

        if (mainCam == null) mainCam = Camera.main;
        if (dragPan == null) dragPan = FindObjectOfType<CameraDragPan>(true);
        if (cameraControll == null) cameraControll = FindObjectOfType<CameraControll>(true);

        StartCoroutine(BeginNextFrame());
    }

    private System.Collections.IEnumerator TimeFreezeWait()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 0f;
    }

    private System.Collections.IEnumerator BeginNextFrame()
    {
        yield return null;

        StoreAndDisableCameraControl();

        if (playerBase != null)
        {
            yield return StartCoroutine(MoveCameraToTargetUnscaled(playerBase.position));
            FrameWorld(playerBase.position);
        }
            


        step = 1;
        overlayRoot.SetActive(true);
        background.SetActive(true);
        stepText.text = "Defend your field base! If it is destroyed, you lose.";
        yield return new WaitForSecondsRealtime(5.0f);

        if (enemyBase != null)
        {
            yield return StartCoroutine(MoveCameraToTargetUnscaled(enemyBase.position));
            FrameWorld(enemyBase.position);
        }
            

        stepText.text = "To win fight, destroy enemy base on the left edge of battlefield!";

        yield return new WaitForSecondsRealtime(5.0f);
        GoStep2_ShowGold();

    }

    private void GoStep2_ShowGold()
    {
        step = 2;

        overlayRoot.SetActive(true);
        stepText.text = "Due to our wonderful workers, gold increase by itself. Spend it to recruit new units. Use it wisely! Its a key to vicotry.";
        if (goldUI) FrameUI(goldUI);

        StartCoroutine(GoStep3_Delayed());
    }

    private System.Collections.IEnumerator GoStep3_Delayed()
    {
        yield return new WaitForSecondsRealtime(7.0f);
        stepText.text = "You can move through map and fight by holding your left mouse button and dragging.";
        yield return new WaitForSecondsRealtime(5.0f);
        StartCoroutine(GoStep3_SpawnFirstUnit());
    }

    private System.Collections.IEnumerator GoStep3_SpawnFirstUnit()
    {
        yield return null;

        step = 3;

        if (unitButtonsContainer == null && buttons != null)
            unitButtonsContainer = buttons.buttonContainer;

        RectTransform targetBtn = null;
        if (unitButtonsContainer != null && unitButtonsContainer.childCount > 0)
            targetBtn = unitButtonsContainer.GetChild(0).GetComponent<RectTransform>();

        overlayRoot.SetActive(true);
        background.SetActive(false);
        stepText.text = "Use hotkey [1] or click unit button, to send you very first unit.";

        if (targetBtn != null)
            FrameUI(targetBtn);

        if (unitSpawner != null)
        {
            unitSpawner.OnUnitSpawned += HandleUnitSpawned;

            
            

        }

        yield return new WaitForSecondsRealtime(5.0f);

        stepText.text = "Units will fight autonomously, just send new units and watch them defeat hordes of enemies (or watch your units get defeated).";
    }

    private void HandleUnitSpawned(int index)
    {
        
        if (step != 3) return;
        if (index != 0) return;

        Time.timeScale = 1f;

        RestoreCameraControl();

        overlayRoot.SetActive(false);
        gameObject.SetActive(false);

        if (unitSpawner != null)
            unitSpawner.OnUnitSpawned -= HandleUnitSpawned;
    }

    private void SkipAll()
    {
        Time.timeScale = 1f;
        RestoreCameraControl();

        overlayRoot.SetActive(false);
        gameObject.SetActive(false);

        if (unitSpawner != null)
            unitSpawner.OnUnitSpawned -= HandleUnitSpawned;
    }

    private void FrameWorld(Vector3 worldPos)
    {
        Vector2 sp = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, sp, null, out var lp);

        highlightFrame.anchoredPosition = lp;
        highlightFrame.sizeDelta = new Vector2(420f, 420f);
    }

    private void FrameUI(RectTransform target)
    {
        Vector3[] wc = new Vector3[4];
        target.GetWorldCorners(wc);

        Vector2 min = WorldToCanvas(wc[0]);
        Vector2 max = WorldToCanvas(wc[2]);

        highlightFrame.anchoredPosition = (min + max) / 2f;
        highlightFrame.sizeDelta = (max - min);
    }

    private Vector2 WorldToCanvas(Vector3 world)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, world), null, out var lp);
        return lp;
    }

    private void StoreAndDisableCameraControl()
    {
        if (mainCam != null) prevCamPos = mainCam.transform.position;

        if(dragPan != null) { prevDragPanEnabled = dragPan.enabled; dragPan.enabled = false; }
        if(cameraControll != null) { prevCameraControllEnabled = cameraControll.enabled; cameraControll.enabled = false; }
    }

    private void RestoreCameraControl()
    {
        if (dragPan != null) dragPan.enabled = prevDragPanEnabled;
        if (cameraControll != null) cameraControll.enabled = prevCameraControllEnabled;
    }

    private System.Collections.IEnumerator MoveCameraToTargetUnscaled(Vector3 worldTarget)
    {
        if (mainCam == null) yield break;

        float endX = worldTarget.x;

        if(dragPan != null)
        {
            endX = Mathf.Clamp(endX, dragPan.minX, dragPan.maxX);
        }
        else
        {
            float camHalfWidth = mainCam.orthographicSize * mainCam.aspect;

            float length = 0f;
            var lb = FindObjectOfType<LevelBuilder>();
            if (lb != null && lb.floor != null)
            {
                length = lb.floor.localScale.x;
            }

            if(length >0f)
            {
                float halfLength = length * 0.5f;
                float minX = -halfLength + camHalfWidth;
                float maxX = halfLength - camHalfWidth;
                endX = Mathf.Clamp(endX, minX, maxX);
            }
        }

        Vector3 start = mainCam.transform.position;
        Vector3 end = new Vector3(endX, start.y, start.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * camLerpSpeed;
            mainCam.transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        mainCam.transform.position = end;
    }
}
