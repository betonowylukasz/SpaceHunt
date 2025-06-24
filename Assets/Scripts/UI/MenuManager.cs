using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private MenuButton playButton;

    private GameObject _currentPanel;

    void Awake()
    {
        Instance = this;
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            _currentPanel = mainMenuPanel;
        }

        playButton.SetText(SaveManager.Instance.CurrentSaveData.saveExists ? "Kontynuuj" : "Nowa gra");
        playButton.onClickEvent.AddListener(() =>
        {
            Debug.Log("Play button clicked");
            if (!SaveManager.Instance.CurrentSaveData.saveExists)
            {
                SaveManager.Instance.CurrentSaveData.saveExists = true;
                SaveManager.Instance.Save();
            }

            SceneManager.LoadScene(SaveManager.Instance.CurrentSaveData.currentLevel == 0 ? "CanteenScene" : "SampleScene");
        });
    }

    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            Debug.Log($"Switching to panel: {panel.name}");
            panel.SetActive(true);
            _currentPanel.SetActive(false);
            _currentPanel = panel;
        }
    }
}
