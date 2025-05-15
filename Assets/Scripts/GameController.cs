using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private GameObject _hub;
    private GameObject _hubInstance;
    public RoomManager RoomManager { get; private set; }
    public ScreenFader ScreenFader;

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

    void Update()
    {
        
    }

    public void LodaLevel(int level)
    {
        if(_hubInstance != null)
        {
            Destroy(_hubInstance);
            _hubInstance = null;
        }

        Debug.Log($"Loading level {level}");
        RoomManager = new RoomManager(5);

        GameObject[] allRooms = Resources.LoadAll<GameObject>($"Rooms/Level{level}");

        foreach (GameObject room in allRooms)
        {
            if (!room.TryGetComponent<Room>(out Room r))
            {
                Debug.LogError($"Room {room.name} is missing a Room component");
                continue;
            }

            RoomManager.AddLevelRoom(room, r.ExitLayout, r.isFinal);
        }

        RoomManager.LoadRoom(null, 0, 0);
    }
}
