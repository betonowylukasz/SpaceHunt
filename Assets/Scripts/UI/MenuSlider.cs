using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class MenuSlider : MonoBehaviour
{
    protected Slider slider;

    protected abstract float GetInitialValue();
    protected abstract void SetValueToTarget(float value);

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
    }

    protected virtual void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);
        slider.value = GetInitialValue();
    }

    protected virtual void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        SetValueToTarget(value);
    }
}
