using UnityEngine;


public class PauseManager : MonoBehaviour
{
    public static PauseManager I { get; private set; }
    public static bool IsPaused => I != null && I.isPaused;

    [SerializeField] private GameObject pauseMenuOverlay;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField, ReadOnly] private bool isPaused;

    private void Awake()
    {
        if(I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[PauseManager] {TutorialState.I != null && TutorialState.I.Active}");
        
        

    }



    private void Update()
    {
        if(pauseMenuOverlay == null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "LoadingScene" && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "VictoryScene")
        {
            
            FindObjectOfType<PauseOverlayConnector>(true).gameObject.SetActive(true);
            

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (TutorialState.I != null && TutorialState.I.Active) return;
            Toggle();
            Debug.Log("[PauseManager] Zmieniono pauzê");
        }
    }

    public void AttachOverlay(GameObject overlayGO, GameObject restartButtonGO, GameObject quitButtonGO)
    {
        Debug.Log("[PauseManager] Przypisano");

        pauseMenuOverlay = overlayGO;
        restartButton = restartButtonGO;
        quitButton = quitButtonGO;
        

        if (pauseMenuOverlay != null) pauseMenuOverlay.SetActive(false);
        SetupButtonsVisibility();

        Toggle();
        Toggle();
    }

    public void DetachOverlay(GameObject overlayGO)
    {
        if(pauseMenuOverlay == overlayGO)
        {
            Debug.Log("[PauseManager] Odpisano");

            pauseMenuOverlay = null;
            restartButton = null;
            quitButton = null;
        }
    }

    private void SetupButtonsVisibility()
    {
        bool isFight = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "FightScene";
        if (restartButton) restartButton.SetActive(isFight);
        if (quitButton) quitButton.SetActive(true);
    }
    public void Toggle()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        //if (TutorialState.I != null && TutorialState.I.Active) return;

        if (isPaused) return;
        isPaused = true;

        if (pauseMenuOverlay) pauseMenuOverlay.SetActive(true);
        Debug.Log("[PauseManager] Zapauzowano");

        SetupButtonsVisibility();

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        if (pauseMenuOverlay) pauseMenuOverlay.SetActive(false);
        Debug.Log("[PauseManager] Odpauzowano");

        Time.timeScale = 1f;
    }

    public void OnClickResume() => Resume();

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void OnClickExitToBase()
    {
        Time.timeScale = 1f;
        RunData.ResetRun();
        MapRunData.Reset();

        PlayerPrefs.Save();

        SceneLoader.LoadScene("MainBaseScene");
    }

    public void OnClickQuitGame()
    {
        try
        {
            MapRunData.Reset();
            RunData.ResetRun();
            PlayerPrefs.Save();
        }
        catch { }

        Time.timeScale = 1f;

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif

    }

}
