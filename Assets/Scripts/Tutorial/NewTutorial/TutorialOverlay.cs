using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button skipButton;

    private System.Action onContinue;
    private System.Action onSkip;


    private void Awake()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => onContinue?.Invoke());

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(() => onSkip?.Invoke());

        Hide();
    }

    public void Hide()
    {
        onContinue = null;
        onSkip = null;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }

    public void Show(string title, string body, System.Action continueAction, System.Action skipAction)
    {
        titleText.text = title;
        bodyText.text = body;

        onContinue = continueAction;
        onSkip = skipAction;

        gameObject.SetActive(true);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

}
