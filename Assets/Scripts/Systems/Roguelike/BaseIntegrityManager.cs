using UnityEngine;

public class BaseIntegrityManager : MonoBehaviour
{
    public static BaseIntegrityManager I { get; private set; }

    private const string PREF_KEY = "run_base_integrity_level";

    [SerializeField] private BaseIntegrityLevel currentLevel = BaseIntegrityLevel.Full;

    public BaseIntegrityLevel CurrentLevel => currentLevel;

    private void Awake()
    {
        if(I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public float GetStartHpMultiplier()
    {
        switch(currentLevel)
        {
            case BaseIntegrityLevel.Full:
                return 1f;

            case BaseIntegrityLevel.Damaged:
                return 0.75f;

            case BaseIntegrityLevel.Critical:
                return 0.5f;

            case BaseIntegrityLevel.Broken:
                return 0.33f;

            default:
                return 1f;
        }
    }

    public bool CanLoseMoreIntegrity()
    {
        return currentLevel != BaseIntegrityLevel.Broken;
    }

    public bool TryApplyLoss()
    {
        if (!CanLoseMoreIntegrity())
            return false;

        currentLevel++;
        Save();

        return true;
    }

    public bool IsRunEndingLoss()
    {
        return currentLevel == BaseIntegrityLevel.Broken;
    }

    public void RepairOneLevel()
    {
        if (currentLevel == BaseIntegrityLevel.Full)
            return;

        currentLevel--;
        Save();
    }

    public void ResetIntegrity()
    {
        currentLevel = BaseIntegrityLevel.Full;
        Save();
    }

    public void SetIntegrityForDebug(BaseIntegrityLevel level)
    {
        currentLevel = level;
        Save();
    }

    public string GetDisplayText()
    {
        int percent = Mathf.RoundToInt(GetStartHpMultiplier() * 100f);
        return $"BASE INTEGRITY: {percent}%";
    }

    private void Save()
    {
        PlayerPrefs.SetInt(PREF_KEY, (int)currentLevel);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        int saved = PlayerPrefs.GetInt(PREF_KEY, 0);
        saved = Mathf.Clamp(saved, 0, 3);

        currentLevel = (BaseIntegrityLevel)saved;
    }

    public int GetSaveValue()
    {
        return (int)currentLevel;
    }

    public void LoadFromSaveValue(int value)
    {
        value = Mathf.Clamp(value, 0, 3);
        currentLevel = (BaseIntegrityLevel)value;
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
    }

    
}
