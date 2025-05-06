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
            _currentPanel.SetActive(false);
            panel.SetActive(true);
            _currentPanel = panel;
        }
    }
}
