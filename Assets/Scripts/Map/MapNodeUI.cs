using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapNodeUI : MonoBehaviour
{
    public TextMeshProUGUI label;
    private MapNodeData data;

    public void Setup(MapNodeData nodeData)
    {
        data = nodeData;
        label.text = data.type.ToString();

        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (data.type == NodeType.Fight || data.type == NodeType.Boss)
        {
            BattleDataCarrier.selectedFight = FightDatabase.instance.allFights[data.fightIndex];
            SceneLoader.LoadScene("LoadingScene");
        }
    }
}
