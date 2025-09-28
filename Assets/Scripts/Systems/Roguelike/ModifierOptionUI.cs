using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ModifierOptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image icon;
    private RunModifierDef def;
    private System.Action<RunModifierDef> onClick;

    public void SetUp(RunModifierDef def, System.Action<RunModifierDef> onClick)
    {
        this.def = def;
        this.onClick = onClick;
        if (nameText != null) nameText.text = def.displayName;
        if (descText != null) descText.text = def.shortDescription;
        if (icon != null && def.icon != null) icon.sprite = def.icon;
    }

    public void OnClick()
    {
        onClick?.Invoke(def);
    }
}