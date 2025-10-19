using UnityEngine;



public class DefeatScreenUI : MonoBehaviour
{
    

    public void ReturnToBase()
    {
        int payout = DefeatPayoutCarrier.materials;
        if(payout > 0)
        {
            RunResources.AddMaterials(payout);
            DefeatPayoutCarrier.materials = 0;
        }

        MapRunData.Reset();
        RunData.ResetRun();

        Time.timeScale = 1f;

        RunSaveManager.Delete();
        RunStatsCollector.Reset();
        MapRunData.pendingNodeId = -1;
        MapRunData.nodeToMarkVisited = -1;

        SceneLoader.LoadScene("MainBaseScene");
    }
}
