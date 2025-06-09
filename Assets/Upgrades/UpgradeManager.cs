using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public InputActionAsset inputActions;  // tutaj przeci¹gasz ca³y asset (Input Actions)

    private InputAction navigateAction;
    private InputAction selectAction;

    private int currentSelectedIndex = 0;

    [Header("Upgrade UI")]
    public GameObject upgradePanel;
    public List<UpgradeCardUI> cardUIs;

    [Header("Available Upgrades")]
    public List<Upgrade> allUpgrades;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        if (inputActions == null) return;

        var uiMap = inputActions.FindActionMap("UI");
        navigateAction = uiMap.FindAction("Navigate");
        selectAction = uiMap.FindAction("Select");

        navigateAction.Enable();
        selectAction.Enable();

        navigateAction.performed += OnNavigate;
        selectAction.performed += OnSelect;
    }

    private void OnDisable()
    {
        if (navigateAction != null)
        {
            navigateAction.performed -= OnNavigate;
            navigateAction.Disable();
        }

        if (selectAction != null)
        {
            selectAction.performed -= OnSelect;
            selectAction.Disable();
        }
    }

    private void Update()
    {
        /*// Nie reagujemy, jeœli panel nieaktywny
        if (!upgradePanel.activeInHierarchy)
            return;

        // Nawigacja w lewo
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedIndex = (currentSelectedIndex - 1 + cardUIs.Count) % cardUIs.Count;
            HighlightCard(currentSelectedIndex);
        }

        // Nawigacja w prawo
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSelectedIndex = (currentSelectedIndex + 1) % cardUIs.Count;
            HighlightCard(currentSelectedIndex);
        }

        // Zatwierdzenie wyboru
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton2)) // X na padzie
        {
            cardUIs[currentSelectedIndex].OnClick();
        }*/
    }

    public void ShowUpgrades()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        List<Upgrade> selected = allUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < cardUIs.Count; i++)
        {
            cardUIs[i].Setup(selected[i]);

            var button = cardUIs[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(cardUIs[i].OnClick);
        }

        // Reset wyboru i podœwietlenia
        currentSelectedIndex = 0;
        HighlightCard(currentSelectedIndex);
    }

    public void SelectUpgrade(Upgrade data)
    {
        PlayerUpgrades.Instance.ApplyUpgrade(data);
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void HighlightCard(int index)
    {
        for (int i = 0; i < cardUIs.Count; i++)
        {
            if (i == index)
            {
                cardUIs[i].transform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                cardUIs[i].transform.localScale = Vector3.one;
            }
        }
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (!upgradePanel.activeInHierarchy)
            return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.x < -0.5f)
        {
            currentSelectedIndex = (currentSelectedIndex - 1 + cardUIs.Count) % cardUIs.Count;
            HighlightCard(currentSelectedIndex);
        }
        else if (input.x > 0.5f)
        {
            currentSelectedIndex = (currentSelectedIndex + 1) % cardUIs.Count;
            HighlightCard(currentSelectedIndex);
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (!upgradePanel.activeInHierarchy)
            return;

        cardUIs[currentSelectedIndex].OnClick();
    }
}
