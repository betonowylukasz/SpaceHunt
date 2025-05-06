using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public string ButtonTextValue = "Button";

    private TMP_Text _buttonText;

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

        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
        else if (exitGame)
        {
            Application.Quit();
        }
    }
}
