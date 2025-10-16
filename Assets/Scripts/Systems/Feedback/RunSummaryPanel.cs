using UnityEngine;
using TMPro;

public class RunSummaryPanel : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] TextMeshProUGUI timeInFights;
    [SerializeField] TextMeshProUGUI winsLosses;
    [SerializeField] TextMeshProUGUI unitsSent;
    [SerializeField] TextMeshProUGUI enemiesKilled;
    [SerializeField] TextMeshProUGUI goldSpent;
    [SerializeField] TextMeshProUGUI dmgDealtTaken;
    [SerializeField] TextMeshProUGUI materials;

    public void Show(string title, int materialsAmountOverride)
    {
        Debug.Log("[RunSummaryPanel] pokazano");
        var s = RunStatsCollector.S;
        header.text = title;

        timeInFights.text = FormatTime(s.timeInFights);
        winsLosses.text = $"{s.fightsWon} / {s.fightsLost}";
        unitsSent.text = s.unitsSpawned.ToString();
        enemiesKilled.text = s.enemiesKilled.ToString();
        goldSpent.text = s.goldSpent.ToString();
        dmgDealtTaken.text = $"{Mathf.RoundToInt(s.totalDamageDealt)} / {Mathf.RoundToInt(s.totalDamageTaken)}";

        int mats = materialsAmountOverride >= 0 ? materialsAmountOverride : s.materialsEarned;
        materials.text = mats.ToString();

        gameObject.SetActive(true);
    }

    string FormatTime(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t) % 60;
        return $"{m:00}:{s:00}";
    }
}
