using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class SkillNodeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Colors")]
    [SerializeField] private Color ownedColor;
    [SerializeField] private Color upgradedColor;


    UnitId unit;
    SkillNode node;
    UnitSkillTreeDef def;
    Image iconImage;
    int level;
    System.Action<UnitId, SkillNode> onBuy;

    public void Setup(UnitId unit, UnitSkillTreeDef def, SkillNode node, int level, System.Action<UnitId, SkillNode> onBuy)
    {
        this.unit = unit;
        this.def = def;
        this.node = node;
        this.level = level;
        this.onBuy = onBuy;

        if (!icon && node.icon) icon.sprite = node.icon;
        iconImage = icon;
        UpdateLevelBadge();

        if(buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(this.unit, this.node));
        }

        
    }

    string ResolveNodeName(string nodeId)
    {
        var pre = def?.nodes?.FirstOrDefault(n => n.nodeId == nodeId);
        return pre != null ? pre.displayName : nodeId;
    }

    void UpdateLevelBadge()
    {
        if (!levelText) return;
        if (node.maxLevel > 1)
        {
            levelText.text = $"Lv {level}/{node.maxLevel}";
            if(level < node.maxLevel && level > 0)
            {
                icon.color = upgradedColor;
            }
            else if (level == node.maxLevel && level > 0)
            {
                icon.color = ownedColor;
            }
        }
        else
        {
            levelText.text = level >= 1 ? "OWNED" : "";
            if(level >= 1)
            {
                icon.color = ownedColor;
            }
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Najechano na node'a");
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.ShowAt(transform as RectTransform, BuildTooltipText(), ModifierTooltip.TooltipSide.Right);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            onBuy?.Invoke(unit, node);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(ModifierTooltip.I != null)
            {
                ModifierTooltip.I.ShowAt(transform as RectTransform, BuildTooltipText());

            }
        }
    }

    public void SetInteractable(bool canBuy)
    {
        if (buyButton) buyButton.interactable = canBuy;
    }

    public void SetDim(bool dim)
    {
        if (!iconImage) return;
        iconImage.color = dim ? new Color(1, 1, 1, 0.35f) : Color.white;
    }


    string BuildTooltipText()
    {
        
        string Hit(int v) => v == 0 ? "" : (v > 0 ? $"+{v}" : $"{v}");

        int cur = level;
        int next = Mathf.Min(cur + 1, node.maxLevel);
        bool maxed = (cur >= node.maxLevel);

        // SUMY (Total) i Delty (Next) – per stat
        // Recruitment
        float totCostPct = (node.reduceRecruitCostPercent) * cur * 100f;
        int totCostFlat = node.addRecruitCostFlat * cur;
        float nxtCostPct = !maxed ? (node.reduceRecruitCostPercent) * 100f : 0f;
        int nxtCostFlat = !maxed ? node.addRecruitCostFlat : 0;

        float totCdPct = (node.reduceRecruitCooldownPercent) * cur * 100f;
        float totCdFlat = node.addRecruitCooldownFlat * cur;
        float nxtCdPct = !maxed ? (node.reduceRecruitCooldownPercent) * 100f : 0f;
        float nxtCdFlat = !maxed ? node.addRecruitCooldownFlat : 0f;

        // Combat
        float totHpPct = (node.addPercentHP) * cur * 100f;
        float totHpFlat = node.addFlatHP * cur;
        float nxtHpPct = !maxed ? (node.addPercentHP) * 100f : 0f;
        float nxtHpFlat = !maxed ? node.addFlatHP : 0;

        float totDmgPct = (node.addPercentDMG) * cur * 100f;
        float totDmgFlat = node.addFlatDMG * cur;
        float nxtDmgPct = !maxed ? (node.addPercentDMG) * 100f : 0f;
        float nxtDmgFlat = !maxed ? node.addFlatDMG : 0;

        float totRangePct = (node.addPercentRange) * cur * 100f;
        float totRangeFlat = node.addFlatRange * cur;
        float nxtRangePct = !maxed ? (node.addPercentRange) * 100f : 0f;
        float nxtRangeFlat = !maxed ? node.addFlatRange : 0f;

        float totAoePct = (node.addPercentAoe) * cur * 100f;
        float totAoeFlat = node.addFlatAoe * cur;
        float nxtAoePct = !maxed ? (node.addPercentAoe) * 100f : 0f;
        float nxtAoeFlat = !maxed ? node.addFlatAoe : 0f;

        int totMulti = node.addMultiStrike * cur;
        int nxtMulti = !maxed ? node.addMultiStrike : 0;

        float totKb = (node.knockbackResist) * cur * 100f;
        float nxtKb = !maxed ? (node.knockbackResist) * 100f : 0f;

        float totRegen = node.regenPerSec * cur;
        float nxtRegen = !maxed ? node.regenPerSec : 0f;

        float totLs = (node.lifeSteal) * cur * 100f;
        float nxtLs = !maxed ? (node.lifeSteal) * 100f : 0f;

        // Budowa treści
        var sb = new System.Text.StringBuilder();

        // Nagłówek + opis
        sb.AppendLine(node.displayName);
        if (!string.IsNullOrEmpty(node.description))
        {
            sb.AppendLine();
            sb.AppendLine(node.description);
        }


        if(node.prerequisites != null && node.prerequisites.Length > 0)
        {
            sb.AppendLine();
            sb.Append("Requires: ");
            for (int i = 0; i < node.prerequisites.Length; i++)
            {
                var preName = ResolveNodeName(node.prerequisites[i]);
                sb.Append(preName);
                if (i < node.prerequisites.Length - 1) sb.Append(", ");
            }

            sb.AppendLine();
        }

        // Helper do składania linii Total/Next
        void AppendPair(string label, float pctTot, float pctNxt, float flatTot, float flatNxt, string unit = "", bool invertSignForPct = false)
        {
            
            float pctTotSign = invertSignForPct ? -pctTot : pctTot;
            float pctNxtSign = invertSignForPct ? -pctNxt : pctNxt;

            bool anyTot = Mathf.Abs(pctTot) > 0.0001f || Mathf.Abs(flatTot) > 0.0001f;
            bool anyNxt = Mathf.Abs(pctNxt) > 0.0001f || Mathf.Abs(flatNxt) > 0.0001f;

            if (anyTot || anyNxt)
            {
                sb.AppendLine();
                sb.Append(label).Append(": ");
                if (anyTot)
                {
                    var parts = new List<string>();
                    if (Mathf.Abs(pctTotSign) > 0.0001f) parts.Add($"{pctTotSign:+0;-0;0}%");
                    if (Mathf.Abs(flatTot) > 0.0001f) parts.Add($"{flatTot:+0.##;-0.##;0}{unit}");
                    sb.Append("Total ").Append(string.Join(" & ", parts));
                }
                if (anyNxt && !maxed)
                {
                    var partsN = new List<string>();
                    if (Mathf.Abs(pctNxtSign) > 0.0001f) partsN.Add($"{pctNxtSign:+0;-0;0}%");
                    if (Mathf.Abs(flatNxt) > 0.0001f) partsN.Add($"{flatNxt:+0.##;-0.##;0}{unit}");
                    if (anyTot) sb.Append(", ");
                    sb.Append("Next ").Append(string.Join(" & ", partsN));
                }
            }
        }

        // Recruitment: cost / cooldown (proc traktujemy jako „reduce” – znak ujemny)
        AppendPair("Cost", totCostPct, nxtCostPct, totCostFlat, nxtCostFlat, "", invertSignForPct: true);
        AppendPair("Cooldown", totCdPct, nxtCdPct, totCdFlat, nxtCdFlat, "s", invertSignForPct: true);

        // Combat: HP / DMG / Range / AOE
        AppendPair("HP", totHpPct, nxtHpPct, totHpFlat, nxtHpFlat);
        AppendPair("DMG", totDmgPct, nxtDmgPct, totDmgFlat, nxtDmgFlat);
        AppendPair("Range", totRangePct, nxtRangePct, totRangeFlat, nxtRangeFlat);
        AppendPair("AOE rad.", totAoePct, nxtAoePct, totAoeFlat, nxtAoeFlat);

        // Multi-Strike / KB / Regen / Lifesteal
        if (totMulti != 0 || nxtMulti != 0)
        {
            sb.AppendLine();
            sb.Append("Multi-Strike: ");
            if (totMulti != 0) sb.Append($"Total {Hit(totMulti)}");
            if (nxtMulti != 0 && !maxed)
            {
                if (totMulti != 0) sb.Append(", ");
                sb.Append($"Next {Hit(nxtMulti)}");
            }
        }
        if (Mathf.Abs(totKb) > 0.0001f || Mathf.Abs(nxtKb) > 0.0001f)
        {
            sb.AppendLine();
            sb.Append("KB Resist: ");
            if (Mathf.Abs(totKb) > 0.0001f) sb.Append($"Total {totKb:+0;-0;0}%");
            if (Mathf.Abs(nxtKb) > 0.0001f && !maxed)
            {
                if (Mathf.Abs(totKb) > 0.0001f) sb.Append(", ");
                sb.Append($"Next {nxtKb:+0;-0;0}%");
            }
        }
        if (Mathf.Abs(totRegen) > 0.0001f || Mathf.Abs(nxtRegen) > 0.0001f)
        {
            sb.AppendLine();
            sb.Append("Regen: ");
            if (Mathf.Abs(totRegen) > 0.0001f) sb.Append($"Total {totRegen:+0.##;-0.##;0} HP/s");
            if (Mathf.Abs(nxtRegen) > 0.0001f && !maxed)
            {
                if (Mathf.Abs(totRegen) > 0.0001f) sb.Append(", ");
                sb.Append($"Next {nxtRegen:+0.##;-0.##;0} HP/s");
            }
        }
        if (Mathf.Abs(totLs) > 0.0001f || Mathf.Abs(nxtLs) > 0.0001f)
        {
            sb.AppendLine();
            sb.Append("Lifesteal: ");
            if (Mathf.Abs(totLs) > 0.0001f) sb.Append($"Total {totLs:+0;-0;0}%");
            if (Mathf.Abs(nxtLs) > 0.0001f && !maxed)
            {
                if (Mathf.Abs(totLs) > 0.0001f) sb.Append(", ");
                sb.Append($"Next {nxtLs:+0;-0;0}%");
            }
        }

        // Enable AOE (informacja)
        if (node.enableAOE)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(cur > 0 ? "AOE: enabled" : "AOE: will be enabled");
        }

        // Koszt i poziom na dole
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine($"Cost: {node.costMaterials} materials");
        sb.Append($"Level: {cur}/{node.maxLevel}");

        return sb.ToString();
    }


}
