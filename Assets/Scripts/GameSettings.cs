using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SettingsManager.Load();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMusicVolume(float volume)
    {
        SettingsManager.CurrentSettings.musicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SettingsManager.CurrentSettings.sfxVolume = volume;
    }

    public float GetMusicVolume()
    {
        return SettingsManager.CurrentSettings.musicVolume;
    }

    public float GetSFXVolume()
    {
        return SettingsManager.CurrentSettings.sfxVolume;
    }

    public void SaveSettings()
    {
        SettingsManager.Save();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitIfNeeded()
    {
        if (Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("GameSettings");
            if (prefab != null)
            {
                GameObject obj = Object.Instantiate(prefab);
                obj.name = "GameSettings";
            }
            else
            {
                Debug.LogError("Brak prefab'u GameSettings w Resources!");
            }
        }
    }
}
