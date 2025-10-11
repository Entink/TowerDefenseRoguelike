using UnityEngine;
using UnityEngine.UI;

public class CommandCenterPanel : MonoBehaviour
{
    [SerializeField] public RectTransform selectionAreaRect;

    [SerializeField] private UnitSelectionManager selectionManager;

    private void OnEnable()
    {
        var unitDB = Resources.Load<UnitDatabase>("UnitDatabase");
        UnitUnlocks.Load(unitDB);
        if(selectionManager != null)
        {
            selectionManager.RefreshConfirmButton();
        }
    }
}
