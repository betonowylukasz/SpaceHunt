using UnityEngine;
using UnityEngine.SceneManagement;

public class CanteenExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveManager.Instance.Save(); // Save the current game state before exiting
            SceneManager.LoadScene("SampleScene");
        }
    }
}
