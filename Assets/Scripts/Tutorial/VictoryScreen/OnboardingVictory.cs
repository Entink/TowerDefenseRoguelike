using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnboardingVictory : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private RectTransform highlightFrame;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private Button skipButton;
    [SerializeField] private RectTransform canvasRect;

    [Header("Targets")]
    [SerializeField] private RectTransform modifierPanelRoot;

    private void Start()
    {
        if(TutorialState.I == null || !TutorialState.I.Active)
        {
            gameObject.SetActive(false);
            return;
        }

        overlayRoot.SetActive(true);
        stepText.text = "Select one modifier (item) - it works only for time of the run";
        if (modifierPanelRoot) FrameUI(modifierPanelRoot);

        if (skipButton) skipButton.onClick.AddListener(SkipAll);
    }

    public void CloseAfterPick()
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
}
