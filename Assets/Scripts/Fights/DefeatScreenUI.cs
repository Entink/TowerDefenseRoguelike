using UnityEngine;



public class DefeatScreenUI : MonoBehaviour
{
    public void ReturnToBase()
    {
        Time.timeScale = 1f;

        SceneLoader.LoadScene("MainBaseScene");
    }
}
