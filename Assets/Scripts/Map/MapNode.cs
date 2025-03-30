using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{
    public FightData fightData;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        BattleDataCarrier.selectedFight = fightData;
        SceneManager.LoadScene("LoadingScene");
    }
}
