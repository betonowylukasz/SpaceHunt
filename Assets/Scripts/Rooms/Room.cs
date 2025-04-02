using UnityEngine;

public class Room : MonoBehaviour
{

    public delegate void RoomExitDelegate(int transitionX, int transitionY);

    [HideInInspector]
    public bool IsLocked { get; set; }

    [HideInInspector]
    public int RoomX = 0;

    [HideInInspector]
    public int RoomY = 0;

    public string ExitLayout = "0000"; //N, E, S, W
    public bool isFinal = false;

    private void Start()
    {
        foreach(RoomExit exit in GetComponentsInChildren<RoomExit>())
        {
            exit.OnPlayerEnter += OnPlayerEnterExit;
        }
    }

    private void OnPlayerEnterExit(int transitionX, int transitionY)
    {
        if (IsLocked)
        {
            return;
        }

        GameController.Instance.RoomManager.LoadRoom(this, transitionX, transitionY);
    }
}
