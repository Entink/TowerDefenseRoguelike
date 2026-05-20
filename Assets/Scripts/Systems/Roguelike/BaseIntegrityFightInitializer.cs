using UnityEngine;

public class BaseIntegrityFightInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (BaseIntegrityManager.I == null)
            return;

        BaseController[] bases = FindObjectsByType<BaseController>(FindObjectsSortMode.None);

        for(int i = 0; i < bases.Length; i++)
        {
            BaseController baseController = bases[i];

            if (baseController == null)
                continue;

            if (!baseController.isPlayerBase)
                continue;

            float hpPercent = BaseIntegrityManager.I.GetStartHpMultiplier();
            baseController.SetCurrentHPPerecnt(hpPercent);

            Debug.Log($"[BaseIntegrity] Player base starts at {Mathf.RoundToInt(hpPercent * 100f)}% HP");
            return;
        }
    }
}
