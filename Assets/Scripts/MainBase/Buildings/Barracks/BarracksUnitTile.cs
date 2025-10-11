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
            bool canUnlockWithMaterials = def.unlockMethod == UnitUnlockMethod.Materials;
            unlockButton.gameObject.SetActive(canUnlockWithMaterials);

            if(canUnlockWithMaterials)
            {
                bool hasReq = UnitUnlocks.HasRequirements(def);
                int cost = Mathf.Max(0, def.costMaterials);

                statusText.text = hasReq ? "Locked" : "";
                costText.text = hasReq ? $"Cost: {cost} materials" : "Requirments not met";
                unlockButton.interactable = hasReq;

                unlockButton.onClick.RemoveAllListeners();
                unlockButton.onClick.AddListener(() =>
                {
                    if (UnitUnlocks.TryUnlockWithMaterials(def))
                        panel.OnUnlocked();
                });
            }
            else
            {
                statusText.text = "Unlock via Event";
                costText.text = "";
                unlockButton.gameObject.SetActive(false);
            }

            
        }
    }
}
