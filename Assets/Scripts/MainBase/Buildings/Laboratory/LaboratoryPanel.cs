using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaboratoryPanel : MonoBehaviour
{
    [SerializeField] private UnitDatabase unitDb;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private UnitSkillTreePanel skillTreePanel;


    private void OnEnable()
    {
        RebuildList();
        if (skillTreePanel) skillTreePanel.gameObject.SetActive(false);
    }
    void RebuildList()
    {
        foreach (Transform ch in listContainer) Destroy(ch.gameObject);

        foreach(var def in unitDb.All)
        {
            if (def == null) continue;
            if (!UnitUnlocks.IsUnlocked(def.id)) continue;

            var go = Instantiate(listItemPrefab, listContainer);
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = def.displayName;

            if(btn)
            {
                btn.onClick.AddListener(() =>
                {
                    if (skillTreePanel)
                    {
                        skillTreePanel.gameObject.SetActive(true);
                        skillTreePanel.Open(def.id);
                    }
                });
            }
        }
    }
}
