using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private float loadingDelay = 1.5f;
    [SerializeField] private string targetSceneName = "FightScene";

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(loadingDelay);
        SceneManager.LoadScene(targetSceneName);
    }
}
