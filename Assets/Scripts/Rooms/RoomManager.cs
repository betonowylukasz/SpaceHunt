using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static RoomManager;

public class RoomManager
{
    public class RoomLayout<K1, K2, T> : Dictionary<K1, Dictionary<K2, T>> {
        public void Add(K1 key1, K2 key2, T value)
        {
            if (!TryGetValue(key1, out Dictionary<K2, T> inner))
            {
                inner = new Dictionary<K2, T>();
                Add(key1, inner);
            }

            inner.Add(key2, value);
        }

        public bool TryGetValue(K1 key1, K2 key2, out T value)
        {
            if (TryGetValue(key1, out Dictionary<K2, T> inner))
            {
                return inner.TryGetValue(key2, out value);
            }

            value = default;
            return false;
        }
    }

    public class RoomData
    {
        public int Exits;
        public string ExitsLayout;
        public GameObject Prefab;
        public int EnemiesToSpawn;
    }

    private readonly bool _isHub;
    private readonly List<RoomData> _levelRooms = new();
    private readonly RoomLayout<int, int, RoomData> _levelLayout = new();
    private GameObject _currentRoom;

    public RoomManager()
    {
        _isHub = true;
    }

    public RoomManager(int roomsCount)
    {
        _isHub = false;
    }

    public void LoadRoom(Room current, int transitionX, int transitionY)
    {
        Debug.Log($"Loading room transX={transitionX}, transY={transitionY}, isHub={_isHub}, current={current}");

        if (current != null)
        {
            current.IsLocked = true;
        }

        if(_isHub)
        {
            GameController.Instance.LodaLevel(1);
            return;
        }

        if(current == null)
        {
            if (_currentRoom != null)
            {
                Debug.Log("Attemting to Load room twice! - return");
                return;
            }

            GenerateLevel(5);

            if(!TryActivateRoom(0, 0, Vector2.zero))
            {
                Debug.LogError("Failed to activate room 0, 0");
            }

            return;
        }

        if(!TryActivateRoom(current.RoomX + transitionX, current.RoomY + transitionY, new Vector2(transitionX, transitionY)))
        {
            current.IsLocked = false;
            Debug.LogError($"Failed to activate room {current.RoomX + transitionX}, {current.RoomY + transitionY}");
        }
    }

    private bool TryActivateRoom(int x, int y, Vector2 entryPoint)
    {
        if (_levelLayout.TryGetValue(x, y, out RoomData roomData))
        {
            GameController.Instance.StartCoroutine(DoRoomTransistion(x, y, entryPoint, roomData));
            return true;
        }

        return false;
    }

    private IEnumerator DoRoomTransistion(int x, int y, Vector2 entryPoint, RoomData roomData)
    {
        yield return GameController.Instance.ScreenFader.FadeOut();

        if (_currentRoom != null && !_currentRoom.IsDestroyed())
        {
            Object.Destroy(_currentRoom);
        }

        GameObject room = Object.Instantiate(roomData.Prefab, Vector3.zero, Quaternion.identity);
        Room roomComponent = room.GetComponent<Room>();

        roomComponent.RoomX = x;
        roomComponent.RoomY = y;

        _currentRoom = room;

        PlayerController.Instance.transform.position = -entryPoint * 3f;

        Debug.Log($"Spawning eneiemies: {roomData.EnemiesToSpawn}");
        if (TrySpawnEnemies(roomData, -entryPoint))
        {
            room.GetComponent<Room>().IsLocked = true;
        }

        yield return GameController.Instance.ScreenFader.FadeIn();
    }

    public void AddLevelRoom(GameObject room, string exitsLayout)
    {
        int exits = 0;

        for (int i = 0; i < exitsLayout.Length; i++)
        {
            if (exitsLayout[i] == '1')
            {
                exits++;
            }
        }

        _levelRooms.Add(new RoomData
        {
            Exits = exits,
            ExitsLayout = exitsLayout,
            Prefab = room,
            EnemiesToSpawn = Random.Range(2, 6)
        });

    }

    private void GenerateLevel(int rooms)
    {
        Debug.Log($"Generating level with {rooms} rooms");
        AppendRoomToLayout(_levelRooms[Random.Range(0, _levelRooms.Count)], 0, 0, ref rooms);
    }

    public void AppendRoomToLayout(RoomData currentRoom, int currentX, int currentY, ref int rooms)
    {
        Debug.Log($"Appending room {currentRoom.Prefab.name} at {currentX}, {currentY} with exits {currentRoom.ExitsLayout}");

        if (rooms <= 0)
        {
            Debug.Log("No more available free rooms");
            return;
        }

        rooms -= currentRoom.Exits;
        _levelLayout.Add(currentX, currentY, currentRoom);

        for (int i = 0; i < currentRoom.Exits; i++)
        {
            for(int j = 0; j < currentRoom.ExitsLayout.Length; j++)
            {
                if (currentRoom.ExitsLayout[j] != '1')
                {
                    continue;
                }

                int nextX = currentX;
                int nextY = currentY;

                switch (j)
                {
                    case 0:
                        nextY++;
                        break;
                    case 1:
                        nextX++;
                        break;
                    case 2:
                        nextY--;
                        break;
                    case 3:
                        nextX--;
                        break;
                }

                if (_levelLayout.TryGetValue(nextX, nextY, out _))
                {
                    continue;
                }

                if (TryGetValidRoom(nextX, nextY, rooms, out RoomData nextRoom, out int forced))
                {
                    rooms += forced;
                    AppendRoomToLayout(nextRoom, nextX, nextY, ref rooms);
                }
                else
                {
                    Debug.LogError($"Failed to get valid room for {nextX}, {nextY}");
                }
            }
        }
    }

    private bool TryGetValidRoom(int x, int y, int exits, out RoomData next, out int forced)
    {
        string targetLayout = "";
        int forcedExits = 0;

        if (_levelLayout.TryGetValue(x, y + 1, out RoomData roomTop))
        {
            targetLayout += roomTop.ExitsLayout[2];

            if (roomTop.ExitsLayout[2] == '1')
            {
                forcedExits++;
            }
        }
        else
        {
            targetLayout += "?";
        }

        if (_levelLayout.TryGetValue(x + 1, y, out RoomData roomRight))
        {
            targetLayout += roomRight.ExitsLayout[3];

            if (roomRight.ExitsLayout[3] == '1')
            {
                forcedExits++;
            }
        }
        else
        {
            targetLayout += "?";
        }

        if (_levelLayout.TryGetValue(x, y - 1, out RoomData roomBottom))
        {
            targetLayout += roomBottom.ExitsLayout[0];

            if (roomBottom.ExitsLayout[0] == '1')
            {
                forcedExits++;
            }
        }
        else
        {
            targetLayout += "?";
        }

        if (_levelLayout.TryGetValue(x - 1, y, out RoomData roomLeft))
        {
            targetLayout += roomLeft.ExitsLayout[1];

            if (roomLeft.ExitsLayout[1] == '1')
            {
                forcedExits++;
            }
        }
        else
        {
            targetLayout += "?";
        }

        Debug.Log($"Target layout for {x}, {y} is {targetLayout}, left exits: {exits}");

        List<RoomData> validRooms = new();

        foreach (RoomData room in _levelRooms.Where(r => r.Exits - forcedExits <= exits))
        {
            if (RoomLayoutMatch(targetLayout, room.ExitsLayout))
            {
                validRooms.Add(room);
            }
        }

        if (validRooms.Count == 0)
        {
            next = null;
            forced = 0;
            return false;
        }

        next = validRooms[Random.Range(0, validRooms.Count)];
        forced = forcedExits;
        return true;
    }

    private bool RoomLayoutMatch(string pattern, string layout)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '?')
            {
                continue;
            }

            if (pattern[i] != layout[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool TrySpawnEnemies(RoomData room, Vector2 shift)
    {
        if(room.EnemiesToSpawn <= 0)
        {
            return false;
        }

        List<Vector2> spawnPoints = GetSpawnPoints(room.EnemiesToSpawn, shift);

        foreach (Vector2 spawnPoint in spawnPoints)
        {
            GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Enemies/soldier/Soldier"), spawnPoint, Quaternion.identity);
            enemy.GetComponent<Enemy>().OnEnemyDeath += () =>
            {
                Debug.Log($"Enemy died, remaining: {room.EnemiesToSpawn}");
                room.EnemiesToSpawn--;
                if (room.EnemiesToSpawn <= 0)
                {
                    Debug.Log($"All enemies in room {room.Prefab.name} are dead, unlocking room");
                    _currentRoom.GetComponent<Room>().IsLocked = false;
                }
            };
        }

        return true;
    }

    private List<Vector2> GetSpawnPoints(int num, Vector2 shift)
    {
        float xMinShift = -3f;
        float xMaxShift = 3f;
        float yMinShift = -3f;
        float yMaxShift = 3f;

        if (shift.x < 0)
        {
            xMinShift = -2f;
        } 
        else if(shift.x > 0)
        {
            xMaxShift = 2f;
        }

        if (shift.y < 0)
        {
            yMinShift = -2f;
        }
        else if (shift.y > 0)
        {
            yMaxShift = 2f;
        }

        List<Vector2> spawnPoints = new();
        for(int i = 0; i < num; i++)
        {
            spawnPoints.Add(new Vector2(Random.Range(xMinShift, xMaxShift), Random.Range(yMinShift, yMaxShift)));
        }

        return spawnPoints;
    }
}
