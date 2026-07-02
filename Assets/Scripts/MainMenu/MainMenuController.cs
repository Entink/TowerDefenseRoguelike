using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button closeSettingsButton;

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        Time.timeScale = 1f;

        CloseSettings();
        startGameButton.onClick.AddListener(EnterBase);
        settingsButton.onClick.AddListener(OpenSettings);
        quitGameButton.onClick.AddListener(QuitGame);
        closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    private void OnDestroy()
    {
        startGameButton.onClick.RemoveListener(EnterBase);
        settingsButton.onClick.RemoveListener(OpenSettings);
        quitGameButton.onClick.RemoveListener(QuitGame);
        closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    public void EnterBase()
    {
        SceneLoader.LoadScene("MainBaseScene");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
