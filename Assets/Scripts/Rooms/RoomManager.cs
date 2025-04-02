using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
        Debug.Log($"Loading room transX={transitionX}, transY={transitionY}, isHub={_isHub}");

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
            GenerateLevel(5);

            if(!TryActivateRoom(0, 0))
            {
                Debug.LogError("Failed to activate room 0, 0");
            }

            return;
        }

        if(!TryActivateRoom(current.RoomX + transitionX, current.RoomY + transitionY))
        {
            current.IsLocked = false;
            Debug.LogError($"Failed to activate room {current.RoomX + transitionX}, {current.RoomY + transitionY}");
        }
    }

    private bool TryActivateRoom(int x, int y)
    {
        if(_currentRoom != null && !_currentRoom.IsDestroyed())
        {
            Object.Destroy(_currentRoom);
        }

        if (_levelLayout.TryGetValue(x, y, out RoomData roomData))
        {
            GameObject room = Object.Instantiate(roomData.Prefab, Vector3.zero, Quaternion.identity);
            Room roomComponent = room.GetComponent<Room>();

            roomComponent.RoomX = x;
            roomComponent.RoomY = y;

            _currentRoom = room;

            PlayerController.Instance.transform.position = Vector3.zero;

            return true;
        }

        return false;
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
            Prefab = room
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
}
