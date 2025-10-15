using UnityEngine;
using UnityEngine.UI;

public class PauseOverlayConnector : MonoBehaviour
{
    [Header("Root & buttons")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitToBaseButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private bool isAttached = false;
    
    private void OnEnable()
    {
        if (PauseManager.I == null) return;
        if (isAttached) return;
        isAttached = true;

        PauseManager.I.AttachOverlay(overlayRoot, restartButton.gameObject, quitButton.gameObject);
        Debug.Log("[PauseOverlayConnector] Podpiêto pauzê");
        if (resumeButton) resumeButton.onClick.AddListener(PauseManager.I.OnClickResume);
        if (restartButton) restartButton.onClick.AddListener(PauseManager.I.OnClickRestart);
        if (exitToBaseButton) exitToBaseButton.onClick.AddListener(PauseManager.I.OnClickExitToBase);
        if (quitButton) quitButton.onClick.AddListener(PauseManager.I.OnClickQuitGame);
    }

    


}
