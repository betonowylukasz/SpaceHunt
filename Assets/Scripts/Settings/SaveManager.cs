using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
    public static SaveManager Instance { get; private set; }
    public SaveData CurrentSaveData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Load();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitIfNeeded()
    {
        if (Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("SaveManager");
            if (prefab != null)
            {
                GameObject obj = Object.Instantiate(prefab);
                obj.name = "SaveManager";
            }
            else
            {
                Debug.LogError("Brak prefab'u SaveManager w Resources!");
            }
        }
    }

    void Load()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Settings loaded.");
        }
        else
        {
            Debug.Log("No settings file found. Using default settings.");
            CurrentSaveData = new SaveData();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(CurrentSaveData, true);
        File.WriteAllText(FilePath, json);
        Debug.Log($"Save file saved to {FilePath}");
    }
}
