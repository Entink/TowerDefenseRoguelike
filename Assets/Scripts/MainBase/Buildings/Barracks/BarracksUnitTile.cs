using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarracksUnitTile : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button unlockButton;

    private UnitDef def;
    private BarracksPanel panel;

    public void Setup(UnitDef def, BarracksPanel panel)
    {
        this.def = def;
        this.panel = panel;

        if (nameText) nameText.text = def.displayName;

        bool unlocked = UnitUnlocks.IsUnlocked(def.id);

        if(unlocked)
        {
            if (statusText) statusText.text = "Unlocked";
            if (costText) costText.text = "";
            if (unlockButton) unlockButton.gameObject.SetActive(false);
        }
        else
        {
            if (statusText) statusText.text = "Locked";

            int cost = def.costMaterials;
            if (costText) costText.text = cost > 0 ? $"Cost: {cost} materials" : "Free";

            if(unlockButton)
            {
                unlockButton.gameObject.SetActive(true);
                unlockButton.onClick.RemoveAllListeners();
                unlockButton.onClick.AddListener(() =>
                {
                    int price = cost;
                    if (price > 0 && RunResources.GetMaterials() < price)
                    {
                        Debug.Log("Not enough materials");
                        return;
                    }

                    if (price > 0) RunResources.AddMaterials(-price);

                    UnitUnlocks.TryUnlockWithMaterials(def);

                    panel.OnUnlocked();
                });
            }
        }
    }
}
