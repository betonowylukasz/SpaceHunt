using UnityEngine;

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class EnemySet
    {
        public GameObject[] enemies;
    }

    public static GameController Instance { get; private set; }

    private GameObject _hub;
    private GameObject _hubInstance;
    public RoomManager RoomManager { get; private set; }
    public ScreenFader ScreenFader;
    public GameObject RoomExitPrefab;

    public EnemySet[] Enemies;

    private int _currentLevel = 0;

    void Awake()
    {
        Instance = this;

        RoomManager = new RoomManager();

        _hub = Resources.Load<GameObject>("Rooms/Room_Hub");
    }

    void Start()
    {
        _hubInstance = Instantiate(_hub, Vector3.zero, Quaternion.identity);
    }

    public void LodaLevel(int level)
    {
        if(_hubInstance != null)
        {
            Destroy(_hubInstance);
            _hubInstance = null;
        }

        RoomManager.UnloadManager();
        _currentLevel = level;

        Debug.Log($"Loading level {level}");
        RoomManager = new RoomManager(Random.Range(5, 10));

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

        RoomManager.LoadRoom(null, 0, 0);
    }

    public void LoadNextLevel()
    {
        if (_currentLevel < 3)
        {
            LodaLevel(_currentLevel + 1);
        }
        else
        {
            Debug.Log("All levels completed!");
            // Handle end of game logic here, e.g., show credits or restart
        }
    }

    public GameObject GetEnemy()
    {
        EnemySet enemies = Enemies[_currentLevel - 1];
        int rng = Random.Range(0, enemies.enemies.Length);

        Debug.Log($"Selected enemy: {enemies.enemies[rng].name} for level {_currentLevel}");

        return enemies.enemies[rng];
    }
}
