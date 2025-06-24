using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanteenExit : MonoBehaviour
{
    public ScreenFader ScreenFader;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveManager.Instance.Save(); // Save the current game state before exiting
            StartCoroutine(FadeOutAndLoadScene());
            //SceneManager.LoadScene("SampleScene");
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        yield return ScreenFader.FadeOut();
        SceneManager.LoadScene("SampleScene");
    }
}
