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

    [Header("Buttons")]
    [SerializeField] private GameObject barracksButton;
    [SerializeField] private GameObject commandCenterButton;
    [SerializeField] private GameObject laboratoryButton;

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
        CloseAll();
        if(TutorialState.I != null && TutorialState.I.Active)
        {
            if (barracksButton) barracksButton.SetActive(false);
            if (commandCenterButton) commandCenterButton.SetActive(true);
            if (laboratoryButton) laboratoryButton.SetActive(false);
            
        }
        else
        {
            if (barracksButton) barracksButton.SetActive(true);
            if (commandCenterButton) commandCenterButton.SetActive(true);
            if (laboratoryButton) laboratoryButton.SetActive(true);
        }
    }
}
