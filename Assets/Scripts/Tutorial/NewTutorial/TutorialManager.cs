using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager I { get; private set; }

    private TutorialOverlay overlay;

    private const string PREF_STEP = "tutorial_step";
    private const string PREF_DONE = "tutorial_done";

    private TutorialStep currentStep;

    private void Awake()
    {
        if(I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        LoadState();
    }

    private void Start()
    {
        FindSceneOverlay();
        TryShowCurrentStep();
        
    }


    private void LoadState()
    {
        bool done = PlayerPrefs.GetInt(PREF_DONE, 0) == 1;

        if(done)
        {
            currentStep = TutorialStep.Completed;
            return;
        }

        int savedStep = PlayerPrefs.GetInt(PREF_STEP, 0);
        currentStep = (TutorialStep)savedStep;
    }


    private void SaveState()
    {
        PlayerPrefs.SetInt(PREF_STEP, (int)currentStep);
        PlayerPrefs.SetInt(PREF_DONE, currentStep == TutorialStep.Completed ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void TryShowCurrentStep()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName != "MainBaseScene" && sceneName != "FightScene" && sceneName != "MapScene")
        {
            return;
        }

        if (currentStep == TutorialStep.Completed)
        {
            overlay.Hide();
            return;
        }

        if(currentStep == TutorialStep.MainBase_Start && sceneName == "MainBaseScene")
        {
            overlay.Show(
                "Welcome to the Base",
                "This is your main base. Start by opening the Command Center (Middle gray building with Radar on top), choosing a unit, and starting your first run.",
                CompleteCurrentStep,
                SkipTutorial
                );

            return;
        }

        if(currentStep == TutorialStep.Map_SelectFirstFight && sceneName == "MapScene")
        {
            overlay.Show(
                "Run Map",
                "This is your map. Here you can choose from available nodes your path to the end of the Wastelands",
                CompleteCurrentStep,
                SkipTutorial);

            return;
        }

        if(currentStep == TutorialStep.Fight_SpawnFirstUnit && sceneName == "FightScene")
        {
            overlay.Show(
                "First Battle!",
                "Send your first unit (Click on it's button at the top). Your goal in each fight is to destroy enemy base (on the left).",
                CompleteCurrentStep,
                SkipTutorial);

            return;
        }

        overlay.Hide();
    }

    public void CompleteCurrentStep()
    {
        if (currentStep == TutorialStep.Completed)
            return;

        currentStep++;

        SaveState();
        TryShowCurrentStep();
    }

    public void SkipTutorial()
    {
        currentStep = TutorialStep.Completed;
        SaveState();

        if (TutorialState.I != null)
            TutorialState.I.Complete();

        if (overlay != null)
            overlay.Hide();
    }

    public void ResetTutorialForDev()
    {
        PlayerPrefs.DeleteKey(PREF_STEP);
        PlayerPrefs.DeleteKey(PREF_DONE);
        PlayerPrefs.Save();

        currentStep = TutorialStep.MainBase_Start;

        if (TutorialState.I != null)
            TutorialState.I.ResetFlagForDev();

        FindSceneOverlay();
        TryShowCurrentStep();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneOverlay();
        TryShowCurrentStep();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FindSceneOverlay()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "MainBaseScene" || sceneName == "FightScene" || sceneName == "MapScene")
        {
            overlay = FindFirstObjectByType<TutorialOverlay>(FindObjectsInactive.Include);
        }
        return;
    }

}
