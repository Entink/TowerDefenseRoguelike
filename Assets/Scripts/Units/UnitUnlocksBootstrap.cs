using UnityEngine;

public class UnitUnlocksBootstrap : MonoBehaviour
{
    public UnitDatabase unitDb;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UnitUnlocks.Load(unitDb);

    }

    private void OnApplicationQuit()
    {
        UnitUnlocks.Save();
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            UnitUnlocks.Save();
            PlayerPrefs.Save();
        }
    }
}
