using UnityEngine;

public class BarracksPanel : MonoBehaviour
{
    [SerializeField] private UnitDatabase unitDb;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject unitTilePrefab;

    public void Rebuild()
    {
        if(!unitDb)
        {
            Debug.LogError("[Barracks] UnitDatabase no assigned.");
            return;
        }

        foreach (Transform ch in container) Destroy(ch.gameObject);

        foreach(var def in unitDb.All)
        {
            if (def == null) continue;
            var go = Instantiate(unitTilePrefab, container);
            var tile = go.GetComponent<BarracksUnitTile>();
            tile.Setup(def, this);
        }
    }

    private void OnEnable()
    {
        Rebuild();
    }
    public void OnUnlocked()
    {
        Rebuild();
    }
}
