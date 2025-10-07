using UnityEngine;

public enum BuildingType
{
    Barracks,
    CommandCenter,
    Laboratory
}
public class MainBaseHub : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject barracksPanel;
    [SerializeField] private GameObject commandCenterPanel;
    [SerializeField] private GameObject laboratoryPanel;

    public void CloseAll()
    {
        if (barracksPanel) barracksPanel.SetActive(false);
        if (commandCenterPanel) commandCenterPanel.SetActive(false);
        if (laboratoryPanel) laboratoryPanel.SetActive(false);
    }

    public void Open(BuildingType type)
    {
        CloseAll();
        switch(type)
        {
            case BuildingType.Barracks: if (barracksPanel) barracksPanel.SetActive(true); break;
            case BuildingType.CommandCenter: if (commandCenterPanel) commandCenterPanel.SetActive(true); break;
            case BuildingType.Laboratory: if (laboratoryPanel) laboratoryPanel.SetActive(true); break;
        }
    }

    private void Start()
    {
        Open(BuildingType.CommandCenter);
    }
}
