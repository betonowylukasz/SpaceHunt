using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField]
    private GameObject mainMenuPanel;
    private GameObject _currentPanel;

    void Awake()
    {
        Instance = this;
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            _currentPanel = mainMenuPanel;
        }
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
