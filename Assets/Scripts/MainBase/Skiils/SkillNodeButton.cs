using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillNodeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;

    UnitId unit;
    SkillNode node;
    int level;
    System.Action<UnitId, SkillNode> onBuy;

    public void Setup(UnitId unit, SkillNode node, int level, System.Action<UnitId, SkillNode> onBuy)
    {
        this.unit = unit;
        this.node = node;
        this.level = level;
        this.onBuy = onBuy;

        if (!icon && node.icon) icon.sprite = node.icon;
        UpdateLevelBadge();

        if(buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(this.unit, this.node));
        }

        bool prereqOk = true;
        if(node.prerequisites != null)
            foreach(var pre in node.prerequisites)
                if(UnitSkillProgress.GetLevel(unit, pre) < 1) { prereqOk = false; break; }

        if (icon) icon.color = prereqOk ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        if (buyButton) buyButton.interactable = prereqOk && level < node.maxLevel;
    }

    void UpdateLevelBadge()
    {
        if (!levelText) return;
        if (node.maxLevel > 1) levelText.text = $"Lv {level}/{node.maxLevel}";
        else levelText.text = level >= 1 ? "Y" : "";

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Najechano na node'a");
        if (ModifierTooltip.I == null) return;
        ModifierTooltip.I.Show(BuildTooltipText());
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
                ModifierTooltip.I.Show(BuildTooltipText());
            }
        }
    }


    string BuildTooltipText()
    {
        int cur = level;
        int next = Mathf.Min(cur + 1, node.maxLevel);

        bool prereqOk = true;
        System.Text.StringBuilder reqSb = new System.Text.StringBuilder();
        if(node.prerequisites != null && node.prerequisites.Length > 0)
        {
            reqSb.Append("Requires: ");
            for(int i = 0; i < node.prerequisites.Length; i++)
            {
                var pre = node.prerequisites[i];
                int preLvl = UnitSkillProgress.GetLevel(unit, pre);
                if (preLvl < 1) prereqOk = false;
                reqSb.Append(pre);
                if (i < node.prerequisites.Length - 1) reqSb.Append(", ");
            }
        }

        bool maxed = (cur >= node.maxLevel);
        int cost = node.costMaterials;

        float totHp = node.addPercentDMG * cur * 100f;
        float totDmg = node.addPercentDMG * cur * 100f;
        float totCd = node.reduceCooldown * cur * 100f;

        float nextHp = (next > cur) ? node.addPercentHP * 100f : 0f;
        float nextDmg = (next > cur) ? node.addPercentDMG * 100f : 0f;
        float nextCd = (next > cur) ? node.reduceCooldown * 100f : 0f;

        var sb = new System.Text.StringBuilder();

        sb.AppendLine(node.displayName);
        sb.AppendLine();

        if(!string.IsNullOrEmpty(node.description))
        {
            sb.AppendLine(node.description);
            sb.AppendLine();
        }

        if(cur > 0)
        {
            sb.Append("Total: ");
            bool any = false;
            if(node.addPercentHP != 0f) { sb.Append($"+{totHp:0}% HP"); any = true; }
            if(node.addPercentDMG != 0f) { if (any) sb.Append(", "); sb.Append($"+{totDmg:0}% DMG"); any = true; }
            if(node.reduceCooldown != 0f) { if (any) sb.Append(", "); sb.Append($"-{totCd:0}% CD"); any = true; }
            if (any) sb.AppendLine();
            
        }

        if(!maxed)
        {
            sb.AppendLine();
            sb.AppendLine($"Cost: {cost} materials");
        }

        if(reqSb.Length > 0)
        {
            sb.AppendLine(reqSb.ToString());
            if (!prereqOk) sb.AppendLine("Prerequisites not met");
        }

        sb.AppendLine();
        sb.AppendLine($"Level: {cur}/{node.maxLevel}");

        return sb.ToString();
    }

}
