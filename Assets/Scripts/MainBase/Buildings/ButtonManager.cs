using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] MainBaseHub hub;

    public void OpenBarracks()
    {
        hub.Open(BuildingType.Barracks);
    }

    public void OpenCommandCenter()
    {
        hub.Open(BuildingType.CommandCenter);
    }

    public void OpenLaboratory()
    {
        hub.Open(BuildingType.Laboratory);
    }
}
