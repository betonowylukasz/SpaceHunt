using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Flags]
    public enum RoomExitLayout
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        NE = North | East,
        NW = North | West,
        SE = South | East,
        SW = South | West,
        NS = North | South,
        EW = East | West,
        NES = North | East | South,
        NEW = North | West | East,
        SEW = South | East | West,
        NESW = North | East | South | West
    }

    [Serializable]
    public class RoomExitDefinition
    {
        public int PositionX;
        public int PositionY;
        public int TransitionX;
        public int TransitionY;
        public bool LevelTransition;

        [HideInInspector]
        public GameObject exit;
    }

    [Serializable]
    public struct EntryPoint
    {
        public int PositionX;
        public int PositionY;
    }

    public delegate void RoomExitDelegate(int transitionX, int transitionY);

    [HideInInspector]
    public bool IsLocked { get; set; }

    [HideInInspector]
    public int RoomX = 0;

    [HideInInspector]
    public int RoomY = 0;

    [HideInInspector]
    public RoomExitLayout ExitDirection = 0;

    public EntryPoint RoomEntryPoint;
    public RoomExitDefinition[] Exits;

    public bool isFinal = false;
    public bool spawnActive = false;

    private GameObject[] _exits;
    private Tilemap tilemapNoCollision;
    private Tilemap tilemapCollision;
    private Tilemap tilemapCollisionLow;
    private Tilemap tilemapDecoration;

    private void Awake()
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        tilemapNoCollision = Array.Find(tilemaps, t => t.name == "Tilemap-NoCollision");
        tilemapCollision = Array.Find(tilemaps, t => t.name == "Tilemap-Collision");
        tilemapCollisionLow = Array.Find(tilemaps, t => t.name == "Tilemap-Collision-Lowlayer");
        tilemapDecoration = Array.Find(tilemaps, t => t.name == "Tilemap-Decorations-NoCollision");

        List<GameObject> exitObjects = new List<GameObject>();

        foreach (RoomExitDefinition exit in Exits) {
            Vector3 position = tilemapCollision.CellToWorld(new Vector3Int(exit.PositionX, exit.PositionY, 0));
            position.x += 0.32f; // Center the exit position on the tile
            position.y += 0.32f; // Center the exit position on the tile

            if(exit.LevelTransition)
            {
                // Empty
            }
            else if (exit.TransitionX == 1)
            {
                position.x -= 0.64f; // Adjust position for East exit
                ExitDirection |= RoomExitLayout.East;
            }
            else if (exit.TransitionX == -1)
            {
                position.x += 0.64f; // Adjust position for West exit
                ExitDirection |= RoomExitLayout.West;
            }
            else if (exit.TransitionY == 1)
            {
                position.y -= 0.64f; // Adjust position for North exit
                ExitDirection |= RoomExitLayout.North;
            }
            else if (exit.TransitionY == -1)
            {
                position.y += 0.64f; // Adjust position for South exit
                ExitDirection |= RoomExitLayout.South;
            }

            GameObject roomExit = Instantiate(GameController.Instance.RoomExitPrefab, position, Quaternion.identity, transform);
            RoomExit roomExitComponent = roomExit.GetComponent<RoomExit>();

            roomExitComponent.TransitionX = exit.TransitionX;
            roomExitComponent.TransitionY = exit.TransitionY;

            roomExitComponent.OnPlayerEnter += exit.LevelTransition ? OnPlayerEnterLevelExit : OnPlayerEnterExit;
            roomExit.SetActive(spawnActive);

            exit.exit = roomExit;
            exitObjects.Add(roomExit);
        }

        _exits = exitObjects.ToArray();
    }

    private void OnPlayerEnterExit(int transitionX, int transitionY)
    {
        if (IsLocked)
        {
            return;
        }

        GameController.Instance.RoomManager.LoadRoom(this, transitionX, transitionY);
    }

    private void OnPlayerEnterLevelExit(int transitionX, int transitionY)
    {
        if (IsLocked)
        {
            return;
        }

        IsLocked = true;
        GameController.Instance.LoadNextLevel();
    }

    public void RoomCleaned()
    {
        IsLocked = false;

        if (!isFinal)
        {
            return;
        }

        RoomExitDefinition exit = Array.Find(Exits, e => e.LevelTransition);
        if (exit == null)
        {
            Debug.LogError("No exit found for Level transition.");
            return;
        }

        tilemapDecoration.SetTile(new Vector3Int(exit.PositionX, exit.PositionY, 0), TileManager.Instance.DoorLevel);
        exit.exit.SetActive(true);
    }

    public void SetLayout(RoomExitLayout layout)
    {
        Debug.Log($"Setting layout for room at ({RoomX}, {RoomY}): {layout}");

        if (layout.HasFlag(RoomExitLayout.North))
        {
            RoomExitDefinition exit = Array.Find(Exits, e => e.TransitionY == 1);
            if (exit == null)
            {
                Debug.LogError("No exit found for North direction.");
            }
            else
            {
                tilemapCollisionLow.SetTile(new Vector3Int(exit.PositionX, exit.PositionY, 0), TileManager.Instance.DoorTop);
                exit.exit.SetActive(true);
            }

        }

        if (layout.HasFlag(RoomExitLayout.East))
        {
            RoomExitDefinition exit = Array.Find(Exits, e => e.TransitionX == 1);
            if (exit == null)
            {
                Debug.LogError("No exit found for East direction.");
                return;
            }
            else
            {
                tilemapCollision.SetTile(new Vector3Int(exit.PositionX, exit.PositionY, 0), TileManager.Instance.DoorSide);
                exit.exit.SetActive(true);
            }

        }

        if (layout.HasFlag(RoomExitLayout.South))
        {
            RoomExitDefinition exit = Array.Find(Exits, e => e.TransitionY == -1);
            if (exit == null)
            {
                Debug.LogError("No exit found for South direction.");
                return;
            }
            else
            {
                tilemapCollision.SetTile(new Vector3Int(exit.PositionX, exit.PositionY, 0), TileManager.Instance.DoorBottom);
                exit.exit.SetActive(true);
            }
        }

        if (layout.HasFlag(RoomExitLayout.West))
        {
            RoomExitDefinition exit = Array.Find(Exits, e => e.TransitionX == -1);
            if (exit == null)
            {
                Debug.LogError("No exit found for West direction.");
                return;
            }
            else
            {
                Vector3Int pos = new Vector3Int(exit.PositionX, exit.PositionY, 0);
                Matrix4x4 rot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));

                tilemapCollision.SetTile(pos, TileManager.Instance.DoorSide);
                tilemapCollision.SetTransformMatrix(pos, rot);
                exit.exit.SetActive(true);
            }
        }
    }

    public List<Vector2> GetValidSpawns(Vector3 playerPos)
    {
        List<Vector2> validSpawns = new List<Vector2>();

        BoundsInt bounds = tilemapNoCollision.cellBounds;
        TileBase[] allTiles = tilemapNoCollision.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector2 tilePos = new Vector2((x + bounds.x) * 0.64f + 0.32f, (y + bounds.y) * 0.64f + 0.32f);

                    float distance = Vector3.Distance(playerPos, tilePos);
                    if (distance > 2f)
                    {
                        Debug.Log($"Valid spawn found at {tilePos} with distance, {bounds.x}, {bounds.y}");
                        validSpawns.Add(tilePos);
                    }
                }
            }
        }

        Debug.Log($"Found {validSpawns.Count} valid spawns, {bounds.size}");
        return validSpawns;
    }
}
