using UnityEngine;
using TMPro;

public class BaseIntegrityUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI integrityText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (integrityText == null)
            return;

        if(BaseIntegrityManager.I == null)
        {
            integrityText.text = "BASE INTEGRITY: ?";
            return;
        }

        integrityText.text = BaseIntegrityManager.I.GetDisplayText();
    }
}
