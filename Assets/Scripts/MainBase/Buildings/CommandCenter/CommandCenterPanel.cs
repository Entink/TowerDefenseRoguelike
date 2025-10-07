using UnityEngine;

public class CommandCenterPanel : MonoBehaviour
{
    [SerializeField] private UnitSelectionManager selectionManager;

    private void OnEnable()
    {
        if(selectionManager != null)
        {
            selectionManager.RefreshConfirmButton();
        }
    }
}
