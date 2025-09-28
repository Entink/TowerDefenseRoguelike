using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainBaseController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI materialsText;

    private int lastKnownMaterials = -1;

    

    private void Update()
    {
        int current = RunResources.GetMaterials();
        if(current != lastKnownMaterials)
        {
            materialsText.text = $"Materials: {current}";
            lastKnownMaterials = current;
        }
    }

    
}
