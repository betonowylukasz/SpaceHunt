using System.IO;
using UnityEngine;

public class SettingsManager
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "settings.json");

    public static SettingsData CurrentSettings { get; private set; } = new SettingsData();

    public static void Save()
    {
        string json = JsonUtility.ToJson(CurrentSettings, true);
        File.WriteAllText(FilePath, json);
        Debug.Log($"Settings saved to {FilePath}");
    }

    public static void Load()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            CurrentSettings = JsonUtility.FromJson<SettingsData>(json);
            Debug.Log("Settings loaded.");
        }
        else
        {
            Debug.Log("No settings file found. Using default settings.");
            CurrentSettings = new SettingsData();
        }
    }
}
