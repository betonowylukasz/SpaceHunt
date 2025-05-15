using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public string ButtonTextValue = "Button";

    private TMP_Text _buttonText;

    [SerializeField]
    private UnityEvent onClickEvent;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private bool exitGame = false;

    void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
        _buttonText.text = ButtonTextValue;
    }

    public void OnClick()
    {
        Debug.Log("Button clicked: " + ButtonTextValue);

        onClickEvent.Invoke();

        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (panel != null)
        {
            MenuManager.Instance.ShowPanel(panel);
        }
        else if (exitGame)
        {
            Application.Quit();
        }
    }
}
