using UnityEngine;

public class TutorialState : MonoBehaviour
{
    public static TutorialState I { get; private set; }

    [SerializeField] private TutorialProfile profile;
    public TutorialProfile Profile => profile;

    private const string PREF_DONE = "tutorial_done";
    public bool Active { get; private set; }

    private void Awake()
    {
        if(I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        Active = PlayerPrefs.GetInt(PREF_DONE, 0) == 0;
    }

    public void Complete()
    {
        Active = false;
        PlayerPrefs.SetInt(PREF_DONE, 1);
        PlayerPrefs.Save();
    }

    public void ResetFlagForDev()
    {
        PlayerPrefs.DeleteKey(PREF_DONE);
        Active = true;
    }

}
