using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEventOptionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI desc;

    private EventOption data;
    private System.Action onClick;

    public void Setup(EventOption opt, System.Action onClick)
    {
        data = opt;
        this.onClick = onClick;
        if (icon) icon.sprite = opt.icon;
        if (label) label.text = opt.label;
        if (desc) desc.text = opt.description;

        button.onClick.RemoveAllListeners();

        bool canAfford = RunResources.GetMaterials() >= opt.minMaterials && RunResources.GetCash() >= opt.minCash;
        button.interactable = canAfford;

        button.onClick.AddListener(() => this.onClick?.Invoke());
    }
}