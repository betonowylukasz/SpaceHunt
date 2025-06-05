using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Room;

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

        public T GetRandom(out K1 k1, out K2 k2)
        {
            if (Count == 0)
            {
                k1 = default;
                k2 = default;
                return default;
            }

            k1 = Keys.ElementAt(Random.Range(0, Count));
            Dictionary<K2, T> inner = this[k1];

            if(inner.Count == 0)
            {
                k2 = default;
                return default;
            }

            k2 = inner.Keys.ElementAt(Random.Range(0, inner.Count));
            return inner[k2];
        }
    }

    public class RoomData
    {
        public Room.RoomExitDefinition[] Exits;
        public GameObject Prefab;
        public bool IsFinal;
    }

    public class LayoutRoom
    {
        public RoomData RoomData;
        public int EnemiesToSpawn;
        public Room.RoomExitLayout Layout = 0;
        public bool HasBoss => EnemiesToSpawn == 0 && RoomData.IsFinal;
    }

    public class AppendQueueItem
    {
        public LayoutRoom currentRoom;
        public int currentX;
        public int currentY;
        public Room.RoomExitLayout add = 0;
    }

    private readonly bool _isHub;
    private readonly List<RoomData> _levelRooms = new();
    private readonly RoomLayout<int, int, LayoutRoom> _levelLayout = new();
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

            if(!TryActivateRoom(0, 0, 0, 0))
            {
                Debug.LogError("Failed to activate room 0, 0");
            }

            return;
        }

        if (!TryActivateRoom(current.RoomX + transitionX, current.RoomY + transitionY, transitionX, transitionY))
        {
            current.IsLocked = false;
            Debug.LogError($"Failed to activate room {current.RoomX + transitionX}, {current.RoomY + transitionY}");
        }
    }

    public void UnloadManager()
    {
        Debug.Log("Unloading RoomManager");
        if (_currentRoom != null)
        {
            Object.Destroy(_currentRoom);
        }
    }

    private bool TryActivateRoom(int x, int y, int transitionX, int transitionY)
    {
        if (_levelLayout.TryGetValue(x, y, out LayoutRoom roomData))
        {
            GameController.Instance.StartCoroutine(DoRoomTransistion(x, y, roomData, transitionX, transitionY));
            return true;
        }

        return false;
    }

    private IEnumerator DoRoomTransistion(int x, int y, LayoutRoom roomData, int transitionX, int transitionY)
    {
        yield return GameController.Instance.ScreenFader.FadeOut();

        if (_currentRoom != null && !_currentRoom.IsDestroyed())
        {
            Object.Destroy(_currentRoom);
        }

        GameObject room = Object.Instantiate(roomData.RoomData.Prefab, Vector3.zero, Quaternion.identity);
        Room roomComponent = room.GetComponent<Room>();
        roomComponent.SetLayout(roomData.Layout);

        roomComponent.RoomX = x;
        roomComponent.RoomY = y;

        _currentRoom = room;

        Room.EntryPoint rep = roomComponent.RoomEntryPoint;
        Vector2 entryPoint = new Vector2(rep.PositionX, rep.PositionY);
        Room.RoomExitDefinition target = System.Array.Find(roomComponent.Exits, e => e.TransitionX == -transitionX && e.TransitionY == -transitionY);
        if (target != null)
        {
            Debug.Log($"Found target exit: {target.PositionX}, {target.PositionY} for transitionX={transitionX}, transitionY={transitionY}");
            entryPoint = new Vector2((target.PositionX + transitionX * 1.5f) * 0.64f + 0.32f, (target.PositionY + transitionY * 1.5f) * 0.64f + 0.32f);
        }

        PlayerController.Instance.transform.position = entryPoint;

        bool hasEnemies = false;
        roomComponent.IsLocked = true;

        Debug.Log($"Spawning eneiemies: {roomData.EnemiesToSpawn}");
        if (TrySpawnEnemies(roomData, entryPoint))
        {
            hasEnemies = true;
        }

        yield return GameController.Instance.ScreenFader.FadeIn();

        if(!hasEnemies)
        {
            Debug.Log($"Room {room.name} has no enemies, unlocking room");
            roomComponent.RoomCleaned();
        }
    }

    public void AddLevelRoom(GameObject room, Room.RoomExitDefinition[] exits, bool isFinal)
    {
        Debug.Log($"Adding room {room.name} with exits {exits.Length}, isFinal={isFinal}");

        _levelRooms.Add(new RoomData
        {
            Exits = exits,
            Prefab = room,
            IsFinal = isFinal
        });

    }

    private void GenerateLevel(int rooms)
    {
        Debug.Log($"Generating level with {rooms} rooms"); 

        RoomData[] validRooms = _levelRooms.Where(r => !r.IsFinal).ToArray();
        LayoutRoom initialRoom = new LayoutRoom
        {
            RoomData = validRooms[Random.Range(0, validRooms.Length)],
            EnemiesToSpawn = 0
        };

        _levelLayout.Add(0, 0, initialRoom);

        rooms -= 1;

        AppendRoomsToLayout(initialRoom, 0, 0, ref rooms, 0, true);
        AddFinalRoom();
    }

    public void AppendRoomsToLayout(LayoutRoom currentRoom, int currentX, int currentY, ref int rooms, Room.RoomExitLayout add = 0, bool initial = false)
    {
        Debug.Log($"Appending room {currentRoom.RoomData.Prefab.name} at {currentX}, {currentY} : {currentRoom.RoomData.Exits.Length}, {rooms}");

        currentRoom.Layout |= add;

        if (rooms <= 0)
        {
            Debug.Log("No more available free rooms");
            return;
        }

        int exits = Random.Range(1, System.Math.Min(currentRoom.RoomData.Exits.Length - (initial ? 0 : 1), rooms) + 1);
        int rng = Random.Range(0, 4);
        Debug.Log($"New rooms to spawn: {exits}");

        Room.RoomExitLayout layout = 0;

        List<AppendQueueItem> queue = new List<AppendQueueItem>();

        for (int i = 0; i < exits; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                int moveIdx = (rng + j) % 4;

                int moveX = 0;
                int moveY = 0;
                Room.RoomExitLayout addExit = 0;

                switch (moveIdx)
                {
                    case 0: // North
                        if(!currentRoom.RoomData.Exits.Any(e => e.TransitionY == 1))
                        {
                            Debug.Log($"No exit to North for room {currentRoom.RoomData.Prefab.name}");
                            continue;
                        }

                        moveY = 1;
                        addExit = Room.RoomExitLayout.South;
                        break;
                    case 1: // East
                        if (!currentRoom.RoomData.Exits.Any(e => e.TransitionX == 1))
                        {
                            Debug.Log($"No exit to East for room {currentRoom.RoomData.Prefab.name}");
                            continue;
                        }

                        moveX = 1;
                        addExit = Room.RoomExitLayout.West;
                        break;
                    case 2: // South
                        if (!currentRoom.RoomData.Exits.Any(e => e.TransitionY == -1))
                        {
                            Debug.Log($"No exit to South for room {currentRoom.RoomData.Prefab.name}");
                            continue;
                        }

                        moveY = -1;
                        addExit = Room.RoomExitLayout.North;
                        break;
                    case 3: // West
                        if (!currentRoom.RoomData.Exits.Any(e => e.TransitionX == -1))
                        {
                            Debug.Log($"No exit to West for room {currentRoom.RoomData.Prefab.name}");
                            continue;
                        }

                        moveX = -1;
                        addExit = Room.RoomExitLayout.East;
                        break;
                }

                if(moveX == 0 && moveY == 0)
                {
                    Debug.LogError("No valid move found, skipping");
                    continue;
                }

                if (_levelLayout.TryGetValue(currentX + moveX, currentY + moveY, out _))
                {
                    Debug.Log($"Room already exists at {currentX + moveX}, {currentY + moveY}");
                    continue;
                }

                Debug.Log($"Adding room at {currentX + moveX}, {currentY + moveY}");

                RoomData[] validRooms = _levelRooms.Where(r => !r.IsFinal && r.Exits.Any(e => e.TransitionX == -moveX && e.TransitionY == -moveY)).ToArray();
                if (validRooms.Length == 0)
                {
                    Debug.LogError($"No valid rooms found for move {moveX}, {moveY}");
                    continue;
                }
                LayoutRoom newRoom = new LayoutRoom
                {
                    RoomData = validRooms[Random.Range(0, validRooms.Length)],
                    EnemiesToSpawn = Random.Range(2, 5)
                };

                _levelLayout.Add(currentX + moveX, currentY + moveY, newRoom);

                rooms -= 1;

                if(moveY == 1)
                {
                    layout |= Room.RoomExitLayout.North;
                }
                else if (moveX == 1)
                {
                    layout |= Room.RoomExitLayout.East;
                }
                else if (moveY == -1)
                {
                    layout |= Room.RoomExitLayout.South;
                }
                else if (moveX == -1)
                {
                    layout |= Room.RoomExitLayout.West;
                }

                queue.Add(new AppendQueueItem
                {
                    currentRoom = newRoom,
                    currentX = currentX + moveX,
                    currentY = currentY + moveY,
                    add = addExit
                });

                break;
            }
        }

        currentRoom.Layout |= layout;

        foreach (AppendQueueItem item in queue)
        {
            AppendRoomsToLayout(item.currentRoom, item.currentX, item.currentY, ref rooms, item.add);
        }
    }

    private bool TrySpawnEnemies(LayoutRoom room, Vector3 playerPos)
    {
        if(room.HasBoss)
        {
            GameObject boss = Object.Instantiate(Resources.Load<GameObject>("Enemies/pirate/Pirate"), Vector3.zero, Quaternion.identity);
            boss.GetComponent<Enemy>().OnEnemyDeath += () =>
            {
                Debug.Log($"Boss died, unlocking room");
                _currentRoom.GetComponent<Room>().RoomCleaned();
            };

            return true;
        }

        if(room.EnemiesToSpawn <= 0)
        {
            return false;
        }

        List<Vector2> spawnPoints = GetSpawnPoints(room, playerPos);

        foreach (Vector2 spawnPoint in spawnPoints)
        {
            GameObject enemy = Object.Instantiate(GameController.Instance.GetEnemy(), spawnPoint, Quaternion.identity);
            enemy.GetComponent<Enemy>().OnEnemyDeath += () =>
            {
                Debug.Log($"Enemy died, remaining: {room.EnemiesToSpawn}");
                room.EnemiesToSpawn--;
                if (room.EnemiesToSpawn <= 0)
                {
                    Debug.Log($"All enemies in room {room.RoomData.Prefab.name} are dead, unlocking room");
                    _currentRoom.GetComponent<Room>().RoomCleaned();
                    UpgradeManager.Instance.ShowUpgrades();
                }
            };
        }

        return true;
    }

    private List<Vector2> GetSpawnPoints(LayoutRoom room, Vector2 playerPos)
    {
        List<Vector2> all = _currentRoom.GetComponent<Room>().GetValidSpawns(playerPos);
        if (all.Count == 0)
        {
            Debug.LogError("No valid spawn points found");
            return new List<Vector2>();
        }

        List<Vector2> spawnPoints = new List<Vector2>();

        for (int i = 0; i < room.EnemiesToSpawn; i++)
        {
            Vector2 spawnPoint = all[Random.Range(0, all.Count)];
            spawnPoints.Add(spawnPoint);
            all.Remove(spawnPoint);
        }

        return spawnPoints;
    }

    private void AddFinalRoom()
    {
        if (_levelRooms.Count == 0)
        {
            return;
        }

        RoomData finalRoom = _levelRooms.Find(r => r.IsFinal);

        if (finalRoom == null)
        {
            Debug.LogError("No final room found");
            return;
        }

        int x, y, freeX = 0, freeY = 0;
        LayoutRoom rand;

        do
        {
            rand = _levelLayout.GetRandom(out x, out y);
            Debug.Log($"Try final: {x}, {y}, {GetFreeSpace(x, y, out freeX, out freeY)}, {freeX}, {freeY}");
        } while (rand != null && !GetFreeSpace(x, y, out freeX, out freeY));

        if(rand == null || (freeX == 0 && freeY == 0))
        {
            Debug.LogError("No random room found");
            return;
        }

        Debug.Log($"Final room: {freeX}, {freeY}");

        Room.RoomExitLayout layout = rand.Layout;
        Room.RoomExitLayout finalLayout = 0;


        if (freeY > y)
        {
            layout |= Room.RoomExitLayout.North;
            finalLayout |= Room.RoomExitLayout.South;
        }
        else if (freeX > x)
        {
            layout |= Room.RoomExitLayout.East;
            finalLayout |= Room.RoomExitLayout.West;
        }
        else if (freeY < y)
        {
            layout |= Room.RoomExitLayout.South;
            finalLayout |= Room.RoomExitLayout.North;
        }
        else if (freeX < x)
        {
            layout |= Room.RoomExitLayout.West;
            finalLayout |= Room.RoomExitLayout.East;
        }

        rand.Layout = layout;

        _levelLayout.Add(freeX, freeY, new LayoutRoom
        {
            RoomData = finalRoom,
            Layout = finalLayout,
            EnemiesToSpawn = 0
        });
    }

    private bool GetFreeSpace(int x, int y, out int freeX, out int freeY)
    {
        _levelLayout.TryGetValue(x, y, out LayoutRoom room);

        freeX = x;
        freeY = y;

        if (!_levelLayout.TryGetValue(x, y + 1, out _) && room.RoomData.Exits.Any(e => e.TransitionY == 1))
        {
            freeY++;
        }
        else if (!_levelLayout.TryGetValue(x + 1, y, out _) && room.RoomData.Exits.Any(e => e.TransitionX == 1))
        {
            freeX++;
        }
        else if (!_levelLayout.TryGetValue(x, y - 1, out _) && room.RoomData.Exits.Any(e => e.TransitionY == -1))
        {
            freeY--;
        }
        else if (!_levelLayout.TryGetValue(x - 1, y, out _) && room.RoomData.Exits.Any(e => e.TransitionX == -1))
        {
            freeX--;
        }
        else
        {
            return false;
        }

        return true;
    }
}
