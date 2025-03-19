using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;


    private void Awake()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError($"{gameObject.name} - CanvasGroup nie zosta³ znaleziony!");
        }

        canvasGroup.alpha = 0;

        
    }

    private void Start()
    {
        UnitController unitController = GetComponentInParent<UnitController>();
        if (unitController)
        {
            HPBarManager.instance.RegisterHPBar(this, unitController.IsAlly);
        }
    }

    public void SetHP(float current, float max)
    {
        hpSlider.value = current / max;
    }

    public void Show()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCanvas(1));
    }

    public void Hide()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCanvas(0));
    }

    

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float duration = 0.5f;
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
