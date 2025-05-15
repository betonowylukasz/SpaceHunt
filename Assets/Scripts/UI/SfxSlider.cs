public class SfxSlider : MenuSlider
{
    protected override float GetInitialValue()
    {
        return GameSettings.Instance.GetSFXVolume();
    }

    protected override void SetValueToTarget(float value)
    {
        GameSettings.Instance.SetSFXVolume(value);
    }
}
