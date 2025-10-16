using UnityEngine;
using TMPro;

public class FightSummaryPanel : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI unitsText;
    [SerializeField] TextMeshProUGUI killsText;
    [SerializeField] TextMeshProUGUI goldSpentText;
    [SerializeField] TextMeshProUGUI dmgText;
    [SerializeField] TextMeshProUGUI matsThisFightText;
    [SerializeField] TextMeshProUGUI matsRunBankedText;

    public void Show(CombatStatsSnapshot s, int matsThisFight, int matsRunBanked)
    {
        Debug.Log("[FightSummaryPanel] pokazano");
        if (s == null) return;
        timeText.text = FormatTime(s.fightTime);
        unitsText.text = s.unitsSpawned.ToString();
        killsText.text = s.enemiesKilled.ToString();
        goldSpentText.text = s.goldSpent.ToString();
        dmgText.text = $"{Mathf.RoundToInt(s.damageDealt)} / {Mathf.RoundToInt(s.damageTaken)}";
        matsThisFightText.text = matsThisFight.ToString();
        matsRunBankedText.text = matsRunBanked.ToString();

        gameObject.SetActive(true);
    }

    private string FormatTime(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t) % 60;
        return $"{m:00}:{s:00}";
    }
}
