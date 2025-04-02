using UnityEngine;

public class RoomExit : MonoBehaviour
{
    public int TransitionX;
    public int TransitionY;
    public event Room.RoomExitDelegate OnPlayerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log($"RoomExit collide : {collision}");
            OnPlayerEnter.Invoke(TransitionX, TransitionY);
        }
    }
}
