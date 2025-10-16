using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnboardingMainBase : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private RectTransform highlightFrame;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button skipButton;

    [Header("Targets")]
    [SerializeField] private Button commandCenterButton;
    [SerializeField] private CommandCenterPanel commandCenterPanel;
    [SerializeField] private UnitSelectionManager selectionManager;
    [SerializeField] private Button startRunButton;

    private int step = 0;
    private int finished = 0;
    private const string finishedKEY = "finished_key";

    private void Start()
    {
        finished = PlayerPrefs.GetInt(finishedKEY, 0);
        if(finished == 1)
        {
            EndOfTutorial();

        }
        else
        {
            if (TutorialState.I == null || !TutorialState.I.Active)
            {
                gameObject.SetActive(false);
                return;
            }

            Begin();

        }


    }

    private void EndOfTutorial()
    {
        overlayRoot.SetActive(true);
        skipButton.onClick.AddListener(SkipAll);
        instructionText.text = "This is the end of the tutorial. Good luck with your journey. [Click skip to end]";
        finished = 2;
        PlayerPrefs.SetInt(finishedKEY, 2);
    }


    private void Begin()
    {
        overlayRoot.SetActive(true);
        skipButton.onClick.AddListener(SkipAll);

        step = 1;
        instructionText.text = "Enter Command Center";
        FrameUI(commandCenterButton.GetComponent<RectTransform>());

        commandCenterButton.onClick.AddListener(OnCommandCenterOpened);
    }

    private void OnCommandCenterOpened()
    {
        if (!commandCenterPanel.gameObject.activeInHierarchy) return;
        step = 2;
        instructionText.text = "Select unit.";
        FrameUI(commandCenterPanel.selectionAreaRect);

        startRunButton.interactable = false;
        selectionManager.OnSelectedUnitsChanged += HandleSelectedUnitsChanged;
    }

    private void HandleSelectedUnitsChanged(int count)
    {
        bool ok = (count == 1);
        startRunButton.interactable = ok;

        if(ok)
        {
            step = 3;
            instructionText.text = "Click Start Run.";
            FrameUI(startRunButton.GetComponent<RectTransform>());
            startRunButton.onClick.AddListener(Complete);
        }
    }

    private void Complete()
    {
        overlayRoot.SetActive(false);
        gameObject.SetActive(false);
    }

    private void SkipAll()
    {
        overlayRoot.SetActive(false);
        gameObject.SetActive(false);
    }

    private void FrameUI(RectTransform target)
    {
        Vector3[] wc = new Vector3[4];
        target.GetWorldCorners(wc);

        RectTransform canvas = overlayRoot.GetComponent<RectTransform>();
        Vector2 min = WorldToCanvas(canvas, wc[0]);
        Vector2 max = WorldToCanvas(canvas, wc[2]);

        highlightFrame.anchoredPosition = (min + max) / 2f;
        highlightFrame.sizeDelta = (max - min);
    }

    private Vector2 WorldToCanvas(RectTransform canvas, Vector3 world)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, RectTransformUtility.WorldToScreenPoint(null, world), null, out pos);
        return pos;
    }

}
