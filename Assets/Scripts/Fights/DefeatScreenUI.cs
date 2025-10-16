using UnityEngine;



public class DefeatScreenUI : MonoBehaviour
{
    

    public void ReturnToBase()
    {
        int payout = PlayerPrefs.GetInt("defeat_payout_materials", 0);
        RunResources.AddMaterials(payout);
        PlayerPrefs.DeleteKey("deafeat_payout_materials");

        MapRunData.Reset();
        RunData.ResetRun();

        Time.timeScale = 1f;

        SceneLoader.LoadScene("MainBaseScene");
    }
}
