using UnityEngine;
using UnityEngine.InputSystem;

public class DeathManager : MonoBehaviour
{
    [SerializeField]
    private GameObject deathMenuUI;
    [SerializeField]
    private GameObject gameUI;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private ScreenFader screenFader;

    public bool IsDead { get; private set; } = false;

    public void PlayerDied()
    {
        SaveManager.Instance.CurrentSaveData.currentLevel = 0;
        SaveManager.Instance.Save();

        IsDead = true;
        gameUI.SetActive(false);
        deathMenuUI.SetActive(true);
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");
        StartCoroutine(screenFader.FadeOut());
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    public void LoadLobby()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("CanteenScene");
    }
}
