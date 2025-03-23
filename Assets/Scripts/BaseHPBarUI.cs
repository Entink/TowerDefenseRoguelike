using UnityEngine;
using UnityEngine.UI;

public class BaseHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    public void SetHP(float current, float max)
    {
        hpSlider.value = current / max;
    }
}
