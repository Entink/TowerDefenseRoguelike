using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class UnitButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int unitIndex;
    public Image cooldownOverlay;
    public TextMeshProUGUI tooltipText;
    public TextMeshProUGUI nameText;

    public GameObject tooltipBox;

    public UnitSpawner spawner;
    public Button button;

    private void Start()
    {
        spawner = FindAnyObjectByType<UnitSpawner>();
        button = GetComponent<Button>();
        tooltipBox.SetActive(false);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            UnitSpawner spawner = FindObjectOfType<UnitSpawner>();
            spawner.TrySpawnUnit(unitIndex);
        });
    }

    private void FixedUpdate()
    {
        string name = spawner.GetUnitName(unitIndex);
        SetLabel(name);
        

        float remaining = spawner.GetCooldownRemaming(unitIndex);
        float total = spawner.GetUnitCooldown(unitIndex);

        if (remaining > 0)
        {
            float percent = remaining / total;
            cooldownOverlay.fillAmount = percent;
            button.interactable = false;
        }
        else
        {
            cooldownOverlay.fillAmount = 0;
            button.interactable = true;
        }

        if(tooltipBox.activeSelf)
        {
            int cost = spawner.GetUnitCost(unitIndex);
            float cd = spawner.GetCooldownRemaming(unitIndex);

            tooltipText.text = $"Cost: {cost}\nCD: {cd:0.0}s";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        tooltipBox.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipBox.SetActive(false);
    }

    public void SetLabel(string name)
    {
        nameText.text = name;
    }
}
