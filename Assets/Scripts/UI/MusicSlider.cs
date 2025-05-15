public class MusicSlider : MenuSlider
{
    protected override float GetInitialValue()
    {
        return GameSettings.Instance.GetMusicVolume();
    }

    protected override void SetValueToTarget(float value)
    {
        GameSettings.Instance.SetMusicVolume(value);
    }
}
