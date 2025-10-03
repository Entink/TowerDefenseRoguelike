using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEventOptionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI desc;


    public void Setup(EventOption opt, System.Action onClick)
    {
        
        if (icon) icon.sprite = opt.icon;
        if (label) label.text = opt.label;

        int haveCash = RunResources.GetCash();
        int haveMat = RunResources.GetMaterials();

        bool hasCash = haveCash >= opt.minCash;
        bool hasMat = haveMat >= opt.minMaterials;
        bool canAfford = hasCash && hasMat;

        if (icon) icon.color = canAfford ? Color.white : new Color(1f, 1f, 1f, 0.5f);

        if (desc)
        {
            if(canAfford)
            {
                desc.text = opt.description;
            }
            else
            {
                string needCash = !hasCash ? "cash" : null;
                string needMats = !hasMat ? "materials" : null;

                string needText = needCash != null && needMats != null
                    ? "Not enough cash and materials"
                    : (needCash ?? $"Not enough {needMats}");


                desc.text = string.IsNullOrWhiteSpace(opt.description)
                    ? $"<color=#FF5555>{needText}</color>"
                    : $"{opt.description}\n<size=80%><color=#FF5555>{needText}</color></size>";

                
            }
        }

        if(button)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = canAfford;
            button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}