using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class EnemySet
    {
        public GameObject[] enemies;
        public GameObject boss;
    }

    public static GameController Instance { get; private set; }

    public RoomManager RoomManager { get; private set; }
    public ScreenFader ScreenFader;
    public GameObject RoomExitPrefab;
    public GameObject BoosHealth;

    public EnemySet[] Enemies;

    private int _currentLevel = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ScreenFader.SetFaded(true);

        if (SaveManager.Instance.CurrentSaveData.currentLevel > 0)
        {
            LoadLevel(SaveManager.Instance.CurrentSaveData.currentLevel, true);
        }
        else
        {
            LoadLevel(1);
        }
    }

    public void LoadLevel(int level, bool useSave = false)
    {
        SaveManager.Instance.CurrentSaveData.currentLevel = level;
        SaveManager.Instance.Save();

        StartCoroutine(DoLevelTransition(level, useSave));
    }

    private IEnumerator DoLevelTransition(int level, bool useSave)
    {
        if (!ScreenFader.isFadedOut) {
            yield return ScreenFader.FadeOut();
        }

        RoomManager?.UnloadManager();
        _currentLevel = level;

        Debug.Log($"Loading level {level}");
        RoomManager = new RoomManager(Random.Range(5 + level, 8 + level * 3));

        GameObject[] allRooms = Resources.LoadAll<GameObject>($"Rooms/Level{level}");

        foreach (GameObject room in allRooms)
        {
            if (!room.TryGetComponent<Room>(out Room r))
            {
                Debug.LogError($"Room {room.name} is missing a Room component");
                continue;
            }

            RoomManager.AddLevelRoom(room, r.Exits, r.isFinal);
        }

        if (useSave)
        {
            RoomManager.LoadSave();
        }
        else
        {
            RoomManager.LoadRoom(null, 0, 0);
        }
    }

    public void LoadNextLevel()
    {
        if (_currentLevel < 3)
        {
            LoadLevel(_currentLevel + 1);
        }
        else
        {
            Debug.Log("All levels completed!");
            SaveManager.Instance.CurrentSaveData.currentLevel = 0;
            SaveManager.Instance.Save();
            SceneManager.LoadScene("EndScene");
        }
    }

    public GameObject GetEnemy()
    {
        EnemySet enemies = Enemies[_currentLevel - 1];
        int rng = Random.Range(0, enemies.enemies.Length);

        Debug.Log($"Selected enemy: {enemies.enemies[rng].name} for level {_currentLevel}");

        return enemies.enemies[rng];
    }

    public GameObject GetBoss()
    {
        EnemySet enemies = Enemies[_currentLevel - 1];
        Debug.Log($"Selected boss: {enemies.boss.name} for level {_currentLevel}");
        return enemies.boss;
    }
}
